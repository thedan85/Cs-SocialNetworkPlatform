using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Model;

public class User : IdentityUser
{
    public override string Id { get; set; } = Guid.NewGuid().ToString();
    public override string UserName { get; set; } = string.Empty;
    public override string? Email { get; set; }
    public override string? PhoneNumber { get; set; }
    public override string? PasswordHash { get; set; }
    
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
