using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Identity;
using SafeVault.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SafeVault.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private ClaimsPrincipal _userClaims;
        private ClaimsPrincipal _adminClaims;
        private ClaimsPrincipal _noroleUser; 

        [SetUp]
        public void Setup()
        {
            // Mock UserManager
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Mock SignInManager
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                _userManagerMock.Object, 
                contextAccessor.Object, 
                claimsFactory.Object, 
                null, null, null, null);

            // Setup test users
            var regularUser = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { 
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Role, "User") // Add User role
                }, "TestAuth"));

            // Admin user
            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { 
                    new Claim(ClaimTypes.Name, "adminuser"),
                    new Claim(ClaimTypes.Role, "Admin") 
                }, "TestAuth"));

            // User without any roles
            var noRoleUser = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, "noroleuser") }, 
                "TestAuth"));
            _userClaims = regularUser;
            _adminClaims = adminUser;
            _noroleUser = noRoleUser;
        }

        [Test]
        public async Task InvalidLoginAttempt_ReturnsPageWithError()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var model = new LoginModel(_signInManagerMock.Object, _userManagerMock.Object)
            {
                Username = "invaliduser",
                Password = "wrongpassword"
            };

            // Act
            var result = await model.OnPostAsync();

            // Assert
            Assert.That(result, Is.InstanceOf<PageResult>());
            Assert.That(model.ModelState.IsValid, Is.False);
            Assert.That(model.ModelState[string.Empty]?.Errors[0].ErrorMessage, 
                Is.EqualTo("Invalid login attempt."));
        }

        [Test]
        public async Task RegularUserAccessingAdminPage_ReturnsForbidden()
        {
            // Arrange
            var adminPage = new AdminDashboardModel()
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext { User = _userClaims }
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>await adminPage.OnGetAsync());
            Assert.That(ex.Message, Is.EqualTo("Access denied. Admin privileges required."));
        }

        [Test]
        public async Task AdminUserAccessingAdminPage_ReturnsSuccess()
        {
            // Arrange
            var adminPage = new AdminDashboardModel()
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext { User = _adminClaims }
                }
            };

            // Act
            var result = await adminPage.OnGetAsync();

            // Assert
            Assert.That(result, Is.InstanceOf<PageResult>());
        }

        [Test]
        public async Task AuthenticatedUserWithoutRole_AccessDenied()
        {
            // Arrange
            var protectedPage = new UserDashboardModel()
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext { User = _noroleUser }
                }
            };

            // Act
           var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>await protectedPage.OnGetAsync());
            Assert.That(ex.Message, Is.EqualTo("Access denied. User privileges required."));

        }

        [Test]
        public async Task UnauthenticatedUserAccess_RedirectsToLogin()
        {
            // Arrange
            var page = new UserDashboardModel()
            {
                PageContext = new PageContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Assert
           var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>await page.OnGetAsync());
            Assert.That(ex.Message, Is.EqualTo("Access denied. User privileges required."));
        }
    }
}