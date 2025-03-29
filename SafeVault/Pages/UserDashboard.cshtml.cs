using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace SafeVault.Pages
{
    [Authorize(Roles = "User")]
    public class UserDashboardModel : PageModel
    {
        public Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("User"))
            {
                throw new UnauthorizedAccessException("Access denied. User privileges required.");
            }
            return Task.FromResult<IActionResult>(Page());
        }
    }
}