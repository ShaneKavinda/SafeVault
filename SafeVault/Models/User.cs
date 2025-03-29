using Microsoft.AspNetCore.Identity;

namespace SafeVault.Models;
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<VaultItem> VaultItems { get; set; }
    }
