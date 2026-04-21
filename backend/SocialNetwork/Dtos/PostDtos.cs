using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class PostCreateRequest
{
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImageUrl { get; set; }

    [StringLength(20)]
    public string? Privacy { get; set; }
}

public class PostUpdateRequest
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImageUrl { get; set; }

    [StringLength(20)]
    public string? Privacy { get; set; }
}

public class PostResponse
{
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Privacy { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsShared { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
