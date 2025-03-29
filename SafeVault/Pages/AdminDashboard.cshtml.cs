using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace SafeVault.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        public Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException("Access denied. Admin privileges required.");
            }
            return Task.FromResult<IActionResult>(Page());
        }
    }
}