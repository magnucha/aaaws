using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace Incremental.Pages;

[AuthorizeForScopes(Scopes = new[] { "User.Read" })]
public class ReadModel : PageModel
    {
    private readonly ITokenAcquisition _tokenAcquisition;
    public string Token { get; set; } = "";

    public ReadModel(ITokenAcquisition tokenAcquisition)
    {
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task OnGet()
    {
        Token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "User.Read" });
    }
}
