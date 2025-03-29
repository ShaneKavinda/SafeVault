using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using SafeVault.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SafeVault.Pages
{
    public class WebFormModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<WebFormModel> _logger;

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        [RegularExpression(ValidationRules.UsernameRegex, ErrorMessage = ValidationRules.UsernameRegexError)]

        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(ValidationRules.PasswordRegex, ErrorMessage = ValidationRules.PasswordRegexError)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [RegularExpression(ValidationRules.PasswordRegex, ErrorMessage = ValidationRules.PasswordRegexError)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Role is required.")]
        public string SelectedRole { get; set; } = string.Empty;

        public WebFormModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            ILogger<WebFormModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
        }

            // Add this property for the role dropdown
        public List<SelectListItem> RoleList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "User", Text = "User" }
        };
        // Add this to the OnGet method
        public void OnGet()
        {
            // Initialize any required data
            if (RoleList.All(r => r.Value != SelectedRole))
            {
                SelectedRole = "User"; // Default value
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!ValidationRules.IsValidEmail(Email))
            {
                ModelState.AddModelError(nameof(Email), "Invalid email address");
                return Page();
            }
            if (!ValidationRules.IsValidPassword(Password))
            {
                ModelState.AddModelError(nameof(Password), ValidationRules.PasswordRegexError);
                return Page();
            }
            if (!ValidationRules.IsValidRole(SelectedRole))
            {
                ModelState.AddModelError(nameof(SelectedRole), "Invalid role selection");
                return Page();
            }

            try
            {
                // Validate role against allowed roles
                var allowedRoles = new[] { "User", "Admin" }; // Define valid roles
                if (!allowedRoles.Contains(SelectedRole))
                {
                    ModelState.AddModelError(string.Empty, "Invalid role selection.");
                    return Page();
                }

                var user = new IdentityUser
                {
                    UserName = Username,
                    Email = Email,
                    EmailConfirmed = false // Require email confirmation
                };

                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    // Add user to selected role
                    await _userManager.AddToRoleAsync(user, SelectedRole);

                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);

                    // Add null check for callbackUrl
                    if (string.IsNullOrWhiteSpace(callbackUrl))
                    {
                        _logger.LogError("Failed to generate confirmation URL for user {UserId}", user.Id);
                        throw new InvalidOperationException("Could not generate confirmation URL");
                    }

                    // Send confirmation email (implement IEmailSender)
                   // Replace the email sending block with this
                    var encodedCallbackUrl = HtmlEncoder.Default.Encode(callbackUrl);
                    await _emailSender.SendEmailAsync(
                        Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{encodedCallbackUrl}'>clicking here</a>.");

                    // Consider logging instead of keeping this in production
                    Console.WriteLine($"Email confirmation link: {callbackUrl}");

                    // Log the user creation but don't auto-sign-in
                    TempData["StatusMessage"] = "Registration successful. Please check your email to confirm your account.";
                    return RedirectToPage("/ConfirmationSent");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, GetUserFriendlyError(error.Code));
                }
            }
            catch (Exception ex)
            {
                // Log the error (implement proper logging)
                _logger.LogError(ex, "Error during user registration");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
            }

            return Page();
        }

        private string GetUserFriendlyError(string errorCode)
        {
            return errorCode switch
            {
                "DuplicateUserName" => "Username is already taken.",
                "DuplicateEmail" => "Email address is already registered.",
                "InvalidEmail" => "Invalid email address.",
                "PasswordTooShort" => "Password does not meet complexity requirements.",
                _ => "An error occurred during registration."
            };
        }
    }
}