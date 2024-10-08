using Microsoft.AspNetCore.Authentication;

namespace NetizenAuthTestBlazorWebApp.AuthProviders.Metamask;

public class MetamaskOptions : RemoteAuthenticationOptions
{
    public MetamaskOptions()
    {
        CallbackPath = new PathString("/signin-metamask");
    }
}
