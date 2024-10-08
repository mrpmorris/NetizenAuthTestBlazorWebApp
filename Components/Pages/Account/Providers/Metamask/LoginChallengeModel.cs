using System.ComponentModel.DataAnnotations;

namespace NetizenAuthTestBlazorWebApp.Components.Pages.Account.Providers.Metamask;

public class LoginChallengeModel
{
    [Required]
    public string Account { get; set; } = null!;

    [Required]
    public string Payload { get; set; } = null!;

    [Required]
    public string Signature { get; set; } = null!;

    public string? ReturnUrl { get; set; }
}
