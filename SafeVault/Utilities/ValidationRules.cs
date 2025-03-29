// Utilities/ValidationRules.cs
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SafeVault.Utilities
{
    public static class ValidationRules
    {
        // Username Rules
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 100;
        public const string UsernameRegex = @"^[a-zA-Z0-9_]+$";
        public const string UsernameRegexError = "Only letters, numbers, and underscores are allowed";
        
        // Password Rules
        public const int PasswordMinLength = 12;
        public const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
        public const string PasswordRegexError = "Must be at least 12 characters with uppercase, lowercase, number, and special character";
        
        // Email Rules
        public const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string EmailRegexError = "Invalid email format";

        // Role Rules
        public static readonly string[] AllowedRoles = { "User", "Admin" };

        // Validation Methods
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, EmailRegex);
        }

        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, PasswordRegex);
        }

        public static bool IsValidRole(string role)
        {
            return AllowedRoles.Contains(role);
        }
    }
}