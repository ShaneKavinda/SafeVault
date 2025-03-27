using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace SafeVault.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        private readonly string _connectionString;

        public string Message { get; set; } = string.Empty;

        public LoginModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public void OnGet()
        {
            // Initialize or reset any data if needed
        }

        public IActionResult OnPost()
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                // Return the page with validation errors
                return Page();
            }

            try
            {
                // Verify the username and password
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password); // Ensure passwords are hashed in production

                    connection.Open();
                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    if (userCount > 0)
                    {
                        // Login successful
                        Message = "Login successful!";
                        return RedirectToPage("Dashboard"); // Redirect to a dashboard or home page
                    }
                    else
                    {
                        // Invalid credentials
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database errors
                ModelState.AddModelError(string.Empty, "An error occurred while verifying your credentials. Please try again.");
                Console.WriteLine(ex.Message);
                return Page();
            }
        }
    }
}