using System.ComponentModel.DataAnnotations;

namespace NetizenAuthTestBlazorWebApp.Components.Pages.Account.Providers.Metamask;

public class LoginModel
{
    [Required, RegularExpression(@"^0x[a-fA-F0-9]{40}$", ErrorMessage = "Invalid account number")]
    public string Account { get; set; } = null!;
}
