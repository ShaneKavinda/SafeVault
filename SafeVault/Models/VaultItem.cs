namespace  SafeVault.Models;

    public class VaultItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
