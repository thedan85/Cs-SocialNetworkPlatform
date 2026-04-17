using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class UserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class UserUpdateRequest
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(500)]
    [Url]
    public string? ProfilePicture { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }

    public bool? IsActive { get; set; }
}
