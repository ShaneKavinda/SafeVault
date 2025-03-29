using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Pages
{
    [AllowAnonymous]  // Important: Allow access even if not logged in
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}