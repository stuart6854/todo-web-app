namespace Domain;

public class User
{
    public Guid Id { get; set; }
    public UserRole Role { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
}