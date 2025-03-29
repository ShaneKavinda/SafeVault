using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using SafeVault.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity.UI.Services;
using SafeVault.Services;
using Moq;

[TestFixture]
public class TestInputValidation
{
    private WebFormModel CreateWebFormModel(string username, string email, string password, string confirmPassword)
    {
        // Arrange common setup
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=SafeVault;Trusted_Connection=True;" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var emailSenderMock = new Mock<IEmailSender>(); // Mock IEmailSender
        var loggerMock = new Mock<ILogger<WebFormModel>>(); // Mock ILogger<WebFormModel>
        var model = new WebFormModel(null, null, emailSenderMock.Object, loggerMock.Object) // Pass mock for emailSender and logger
        {
            Username = username,
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword
        };

        return model;
    }

    [Test]
    public void TestForSQLInjection()
    {
        // Arrange
        string maliciousInput = "'; DROP TABLE Users; --";
        var model = CreateWebFormModel(maliciousInput, "test@example.com", "SecurePassword123!", "SecurePassword123!");

        // Act
        var result = model.OnPostAsync();

        // Assert
        Assert.That(result, Is.InstanceOf<PageResult>(), "SQL Injection attempt should not succeed.");
        Assert.That(model.ModelState.IsValid, Is.False, "Model state should be invalid for SQL injection.");
    }

    [Test]
    public void TestForXSS()
    {
        // Arrange
        string maliciousInput = "<script>alert('XSS')</script>";
        var model = CreateWebFormModel(maliciousInput, "test@example.com", "SecurePassword123!", "SecurePassword123!");

        // Act
        var result = model.OnPostAsync();

        // Assert
        Assert.That(result, Is.InstanceOf<PageResult>(), "XSS attempt should not succeed.");
        Assert.That(model.ModelState.IsValid, Is.False, "Model state should be invalid for XSS.");
    }
}