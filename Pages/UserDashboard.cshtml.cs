using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages
{
    [Authorize(Roles = "User")]
    public class UserDashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}