using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class RegisterRequest
{
    [Required]
    [StringLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ProfilePicture { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }
}

public class LoginRequest
{
    [Required]
    [StringLength(256)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;
}

public class AuthUserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
}
