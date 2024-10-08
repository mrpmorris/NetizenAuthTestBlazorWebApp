namespace NetizenAuthTestBlazorWebApp.AuthProviders.Metamask;

/// <summary>
/// Default values for Metamask authentication
/// </summary>
public static class MetamaskDefaults
{
    /// <summary>
    /// The default scheme for Metamask authentication. Defaults to <c>Metamask</c>.
    /// </summary>
    public const string AuthenticationScheme = "metamask";

    /// <summary>
    /// The default display name for Metamask authentication. Defaults to <c>Metamask</c>.
    /// </summary>
    public static readonly string DisplayName = "Metamask";

    /// <summary>
    /// The default endpoint used to perform Metamask authentication.
    /// </summary>
    public static readonly string AuthorizationEndpoint = "/account/provider/metamask/login";
}
