using Microsoft.AspNetCore.Authentication;

namespace NetizenAuthTestBlazorWebApp.AuthProviders.Metamask;

public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds Metamask authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="MetamaskDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Metamask authentication allows application users to sign in with their Metamask account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddMetamask(this AuthenticationBuilder builder)
        => builder.AddMetamask(MetamaskDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Adds Metamask authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="MetamaskDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Metamask authentication allows application users to sign in with their Metamask account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="MetamaskOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddMetamask(this AuthenticationBuilder builder, Action<MetamaskOptions> configureOptions)
        => builder.AddMetamask(MetamaskDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Adds Metamask authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="MetamaskDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Metamask authentication allows application users to sign in with their Metamask account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="MetamaskOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddMetamask(this AuthenticationBuilder builder, string authenticationScheme, Action<MetamaskOptions> configureOptions)
        => builder.AddMetamask(authenticationScheme, MetamaskDefaults.DisplayName, configureOptions);

    /// <summary>
    /// Adds Metamask authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="MetamaskDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Metamask authentication allows application users to sign in with their Metamask account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="MetamaskOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddMetamask(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<MetamaskOptions> configureOptions) =>
        builder.AddRemoteScheme<MetamaskOptions, MetamaskHandler>(authenticationScheme, displayName, configureOptions);
}
