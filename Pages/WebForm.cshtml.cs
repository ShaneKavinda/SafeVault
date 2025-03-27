using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;

namespace SafeVault.Pages
{
    public class WebFormModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        private readonly string _connectionString;

        public string Message { get; set; } = string.Empty;

        public WebFormModel(IConfiguration configuration)
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
                // Save to the database
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    // use a parametrized query to prevent SQL injection
                    string query = "INSERT INTO Users (Username, Email) VALUES (@Username, @Email);";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Email", Email);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                // Set success message and redirect to a success page
                Message = "Form submitted successfully!";
                return RedirectToPage("Success");
            }
            catch (Exception ex)
            {
                // Handle database errors
                ModelState.AddModelError(string.Empty, "An error occurred while saving your data. Please try again.");
                Console.WriteLine(ex.Message);
                return Page();
            }
        }
    }
}