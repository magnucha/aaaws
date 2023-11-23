using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace _04_DefaultIdentity.Pages;

public class IndexModel : PageModel
{
    public List<User> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        // var credential = new DefaultAzureCredential();
        var credential = new ChainedTokenCredential(
            new AzureCliCredential(),
            new DefaultAzureCredential()
        );
        var graphClient = new GraphServiceClient(credential);

        var result = await graphClient.Users.GetAsync();
        if (result?.Value != null)
        {
            Users = result.Value;
        }
    }
}
