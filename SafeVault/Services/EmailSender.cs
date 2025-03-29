// Add to the top of your file
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

// Implementation
namespace SafeVault.Services{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implement actual email sending logic here
            // For now, just log the email content
            System.Console.WriteLine($"Sending email to: {email}");
            System.Console.WriteLine($"Subject: {subject}");
            System.Console.WriteLine($"HTML Content: {htmlMessage}");
            
            return Task.CompletedTask;
        }
    }
}
