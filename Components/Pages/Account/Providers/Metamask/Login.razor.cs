using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Web;

namespace NetizenAuthTestBlazorWebApp.Components.Pages.Account.Providers.Metamask;

public partial class Login
{
    [SupplyParameterFromForm]
    public required LoginModel Model { get; set; }

    [SupplyParameterFromQuery(Name = "ReturnUrl")]
    public string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery(Name = "Accounts")]
    public string? QuerystringAccounts { get; set; }

    private string[] Accounts = [];

    private EditForm EditForm = null!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!string.IsNullOrWhiteSpace(QuerystringAccounts))
            Accounts = QuerystringAccounts.Split(',');
        Model ??= new();
    }
    private void Save()
    {
        bool isValid = EditForm.EditContext!.Validate();
        if (!isValid)
            return;

        var queryString = new QueryString();
        queryString = queryString.Add("account", Model.Account);
        queryString = queryString.Add("returnUrl", ReturnUrl ?? "");
        NavigationManager.NavigateTo("/account/provider/metamask/login-challenge" + queryString);
    }
}
