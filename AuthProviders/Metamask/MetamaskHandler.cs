using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace NetizenAuthTestBlazorWebApp.AuthProviders.Metamask;

public class MetamaskHandler : RemoteAuthenticationHandler<MetamaskOptions>
{
    private readonly IDataProtectionProvider DataProtectionProvider;
    private readonly IMemoryCache MemoryCache;

    public MetamaskHandler(
        IOptionsMonitor<MetamaskOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IDataProtectionProvider dataProtectionProvider,
        IMemoryCache memoryCache)
        : base(options, logger, encoder)
    {
        DataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (string.IsNullOrEmpty(properties.RedirectUri))
        {
            properties.RedirectUri = OriginalPathBase + OriginalPath + Request.QueryString;
        }
        GenerateCorrelationId(properties);

        properties.Items.TryGetValue("ReturnUrl", out string? returnUrl);
        Context.Response.Redirect($"/account/provider/metamask/login?returnUrl={HttpUtility.UrlEncode(returnUrl)}");
        return Task.CompletedTask;
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        try
        {
            // Ensure the request method is POST
            if (!string.Equals(Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
                return HandleRequestResult.Fail("Invalid request method. POST required.");

            // Ensure the request has form content
            if (!Request.HasFormContentType)
                return HandleRequestResult.Fail("Invalid content type. Form content required.");

            // Read form data asynchronously
            var form = await Request.ReadFormAsync();

            // Extract form values
            string account = form["account"];
            string payload = form["payload"];
            string signature = form["signature"];
            string returnUrl = form["returnUrl"];

            // Validate extracted values
            if (string.IsNullOrWhiteSpace(account) ||
                string.IsNullOrWhiteSpace(payload) ||
                string.IsNullOrWhiteSpace(signature))
                return HandleRequestResult.Fail("Missing required form data.");

            // Proceed with your authentication logic
            // For example, validate the signature
            string? validationError = GetSignatureErrorMessage(account, payload, signature);
            if (!string.IsNullOrEmpty(validationError))
            {
                // Optionally, you can set the failure message in the AuthenticationProperties
                var properties = new AuthenticationProperties
                {
                    RedirectUri = "/Account/Login-Challenge?error=" + Uri.EscapeDataString(validationError)
                };
                return HandleRequestResult.Fail(validationError, properties);
            }

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(type: ClaimTypes.NameIdentifier, value: account, Scheme.Name, Scheme.Name),
                new Claim(ClaimTypes.Name, account, Scheme.Name, Scheme.Name),
            };

            // Create the identity and principal
            var identity = new ClaimsIdentity(claims, ClaimsIssuer);
            var principal = new ClaimsPrincipal(identity);

            // Create authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                RedirectUri = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl
            };

            // Create the authentication ticket
            var ticket = new AuthenticationTicket(principal, authProperties, Scheme.Name);

            // Return success result
            return HandleRequestResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An exception occurred during Metamask authentication.");
            return HandleRequestResult.Fail(ex);
        }
    }


    private IDataProtector GetDataProtector() => DataProtectionProvider.CreateProtector("LoginChallenge");


    public string? GetSignatureErrorMessage(string account, string payload, string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            return "Required";

        string currentChallengePayload = GetOrCreateChallengePayload(account);

        ChallengeContent signedContent = DecodePayload(payload);
        if (signedContent.HasExpired() || payload != currentChallengePayload)
            return "Timed out. Please try again.";

        bool areSame = false;
        try
        {
            var signer = new Nethereum.Signer.EthereumMessageSigner();
            string signingWalletAddress = signer.EncodeUTF8AndEcRecover(payload, signature);

            areSame = signingWalletAddress.Equals(signedContent.Account, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            areSame = false;
        }
        if (!areSame)
            return "Invalid signature.";
        return null;
    }

    private string GetOrCreateChallengePayload(string account)
    {
        string ipAddress = Context.Connection.RemoteIpAddress!.ToString();
        bool hasCachedValue = MemoryCache.TryGetValue(ipAddress, out object? cachedValue);

        if (hasCachedValue)
        {
            string cachedPayload = (string)cachedValue!;
            ChallengeContent decodedChallengeContent = DecodePayload(cachedPayload);

            if (!decodedChallengeContent.HasExpired())
                return cachedPayload;
        }

        var challengeContent =
                new ChallengeContent(
                    DateTimeOffset.Now.AddMinutes(5),
                    IPAddress: Context.Connection.RemoteIpAddress.ToString(),
                    Account: account
                );

        string encodedChallengeContent = CreatePayloadFromChallenge(challengeContent);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(challengeContent.ExpiresAfter);
        MemoryCache.Set(ipAddress, encodedChallengeContent, cacheEntryOptions);

        return encodedChallengeContent;
    }

    private string CreatePayloadFromChallenge(ChallengeContent challengeContent)
    {
        string result = JsonSerializer.Serialize(challengeContent);
        byte[] protectedBytes = GetDataProtector().Protect(System.Text.Encoding.UTF8.GetBytes(result));
        return Convert.ToBase64String(protectedBytes);
    }

    private ChallengeContent DecodePayload(string encodedPayload)
    {
        byte[] encodedPayloadBytes = Convert.FromBase64String(encodedPayload);
        byte[] unprotectedPayloadBytes = GetDataProtector().Unprotect(encodedPayloadBytes);
        string json = System.Text.Encoding.UTF8.GetString(unprotectedPayloadBytes);
        var result = JsonSerializer.Deserialize<ChallengeContent>(json)!;
        return result;
    }


    private record ChallengeContent(DateTimeOffset ExpiresAfter, string IPAddress, string Account)
    {
        public bool HasExpired() => ExpiresAfter <= DateTimeOffset.UtcNow;
    }
}
