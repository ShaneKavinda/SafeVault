using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace SafeVault.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("DefaultConnection", "Connection string cannot be null.");
        }

        [HttpPost]
        public IActionResult Submit(string username, string email)
        {
            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                ViewBag.Message = "Username and Email are required.";
                return View("Index");
            }

            // Save to database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Username, Email) VALUES (@Username, @Email)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                command.ExecuteNonQuery();
            }

            ViewBag.Message = "Form submitted successfully!";
            return View("Success");
        }
    }
}