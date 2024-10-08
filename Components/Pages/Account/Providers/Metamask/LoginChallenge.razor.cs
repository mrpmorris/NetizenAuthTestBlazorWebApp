using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace NetizenAuthTestBlazorWebApp.Components.Pages.Account.Providers.Metamask;

public partial class LoginChallenge
{
    [SupplyParameterFromQuery(Name = "Account")]
    public required string Account { get; set; }

    [SupplyParameterFromQuery(Name = "ReturnUrl")]
    public required string? ReturnUrl { get; set; }

    [SupplyParameterFromForm]
    public LoginChallengeModel Model { get; set; } = null!;

    private EditForm EditForm = null!;
    private bool ShouldRedirect;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Model ??= new();
        Model.Account ??= Account;
        Model.Payload ??= GetOrCreateChallengePayload();
        if (string.IsNullOrWhiteSpace(ReturnUrl)) ReturnUrl = "/";
    }

    private IDataProtector GetDataProtector() => DataProtectionProvider.CreateProtector("LoginChallenge");

    private async Task SaveAsync()
    {
        bool isValid = EditForm.EditContext!.Validate();
        if (!isValid)
            return;

        Model.ReturnUrl = ReturnUrl;
        ShouldRedirect = true;
    }

    private string GetOrCreateChallengePayload()
    {
        string ipAddress = HttpContextAccessor.HttpContext!.Connection!.RemoteIpAddress!.ToString();
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
                    IPAddress: HttpContextAccessor.HttpContext!.Connection!.RemoteIpAddress!.ToString(),
                    Account: Account
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