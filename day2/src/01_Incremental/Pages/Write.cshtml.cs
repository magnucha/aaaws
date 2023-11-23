using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace Incremental.Pages;

[AuthorizeForScopes(Scopes = new[] { "User.ReadWrite" })]
public class WriteModel : PageModel
{

    private readonly ITokenAcquisition _tokenAcquisition;
    public string Token { get; set; } = "";

    public WriteModel(ITokenAcquisition tokenAcquisition)
    {
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task OnGet()
    {
        Token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "User.ReadWrite" });
    }
}

