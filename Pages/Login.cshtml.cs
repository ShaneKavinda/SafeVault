using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using SafeVault.Utilities;

namespace SafeVault.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(ValidationRules.UsernameMaxLength, 
            ErrorMessage = "Username cannot exceed {1} characters")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(ValidationRules.PasswordRegex, 
            ErrorMessage = ValidationRules.PasswordRegexError)]
        public string Password { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet()
        {
            // Initialize or reset any data if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Replaced RAW SQL with parameterized queries to prevent SQL injection
            // This is handled by ASP.NET Identity.
            // The SignInManager will handle the password hashing and validation securely.
            // Check if the user exists
            var result = await _signInManager.PasswordSignInAsync(
                Username,
                Password,
                isPersistent: false,
                lockoutOnFailure: false);

             if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(Username);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                    return RedirectToPage("/AdminDashboard");
                    
                return RedirectToPage("/UserDashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}