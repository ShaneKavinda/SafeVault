using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using SafeVault.Pages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Castle.Components.DictionaryAdapter.Xml;

[TestFixture]
public class TestInputValidation {
    [Test]
    public void TestForSQLInjection() {
        // Arrange
        var inMemorySettings = new Dictionary<string, string> {
            { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=SafeVault;Trusted_Connection=True;" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var model = new WebFormModel(configuration);
        string maliciousInput = "'; DROP TABLE Users; --";
        model.Username = maliciousInput;
        model.Email = "test@example.com";

        // Act
        var result = model.OnPost();

        // Assert
        Assert.That(result, Is.InstanceOf<PageResult>(), "SQL Injection attempt should not succeed.");
        Assert.That(model.ModelState.IsValid, Is.False, "Model state will be invalid");
    }

    [Test]
    public void TestForXSS() {
        // Arrange
        var inMemorySettings = new Dictionary<string, string> {
            { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=SafeVault;Trusted_Connection=True;" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var model = new WebFormModel(configuration);
        string maliciousInput = "<script>alert('XSS')</script>";
        model.Username = maliciousInput;
        model.Email = "test@example.com";

        // Act
        var result = model.OnPost();

        // Assert
        Assert.That(result, Is.InstanceOf<PageResult>(), "XSS attempt should not succeed.");
        Assert.That(model.ModelState.IsValid, Is.False, "Model state will be invalid");
    }

}