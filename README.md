# SafeVault 🔒

An ASP.NET Core web application with secure authentication and authorization features, integrated with MySQL for data storage.

![SafeVault Screenshot](./screenshot.png) <!-- Add a screenshot later if needed -->

## Features

- User registration/login with ASP.NET Core Identity
- Role-based access control (Admin/User)
- Email confirmation workflow
- Secure password policies
- MySQL database integration
- Unit and integration tests

---

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) (8.0+ recommended)
- [Git](https://git-scm.com/)
- An email service account (or use dummy credentials for development)

---

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/SafeVault.git
cd SafeVault
```

### 2. Database Setup
Create a MySQL database:
```sql
CREATE DATABASE safevault;
```
Run the .sql queries in database.sql to add the User table.

Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=safevault;User=root;Password=yourpassword;"
  }
}
```

### 3. Apply Database Migrations
Run the following command to apply migrations:
```bash
dotnet ef database update
```

### 4. Configure Email Settings (Optional)
Create `appsettings.Development.json` for local email configuration:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "your@email.com",
    "Password": "yourpassword"
  }
}
```

---

## Project Structure
```plaintext
SafeVault/
├── Pages/              # Razor Pages
├── Services/           # Business logic
├── Data/               # Database context
├── Utilities/          # Helper classes
├── SafeVault.Tests/    # Test project
├── appsettings.json    # Configuration
└── Program.cs          # Startup configuration
```

---

## Running the Application
Build and run the application:
```bash
dotnet build
dotnet run
```

Visit [http://localhost:5022](http://localhost:5022) in your browser.

---

## Testing
Run tests from the root directory:
```bash
dotnet test SafeVault.Tests
```

### Test Scenarios
- Invalid login attempts
- Unauthorized access checks
- Registration validation
- Role-based authorization

---

## Configuration

| Setting               | Environment Variable       | Default                  |
|-----------------------|---------------------------|--------------------------|
| Database Connection   | `SAFEVAULT_DB_CONNECTION` | From `appsettings.json` |
| Email Service         | `SAFEVAULT_EMAIL_*`       | Development settings     |
| Admin Credentials     | `SAFEVAULT_ADMIN_EMAIL/PASS` | Not set               |

---
