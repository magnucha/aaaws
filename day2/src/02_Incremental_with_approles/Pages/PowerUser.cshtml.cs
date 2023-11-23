using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Incremental.Pages
{
    [Authorize(Roles = "PowerUser")]
    public class PowerUser : PageModel
    {

        public PowerUser()
        {
        }

        public void OnGet()
        {
        }
    }
}