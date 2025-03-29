using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data; // Ensure this namespace matches your AppDBContext location
using SafeVault.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options => 
    {
        // Secure all pages by default
        options.Conventions.AuthorizeFolder("/");
        
        // Allow anonymous access to specific pages
        options.Conventions.AllowAnonymousToPage("/Login");
        options.Conventions.AllowAnonymousToPage("/Register");
        options.Conventions.AllowAnonymousToPage("/ConfirmEmail");
        options.Conventions.AllowAnonymousToPage("/ConfirmationSent");
    }
);

builder.Services.ConfigureApplicationCookie(options => 
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";  // This matches our new page
});

builder.Services.AddSession(); // Add session support

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure the database context
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDBContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    
    options.User.RequireUniqueEmail = true;
});

// Add Email Sender
builder.Services.AddSingleton<IEmailSender, EmailSender>(); // Implement IEmailSender

// Role seeding function
async Task SeedRoles(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    string[] roleNames = { "Admin", "User" };
    
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

var app = builder.Build();

// Add this BEFORE any other middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Seed roles during application startup
using (var scope = app.Services.CreateScope())
{
    await SeedRoles(scope.ServiceProvider);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Add this - authentication must come before authorization
app.UseAuthorization();

app.MapRazorPages();

app.Run();