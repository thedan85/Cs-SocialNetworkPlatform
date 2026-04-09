using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class PostCreateRequest
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImageUrl { get; set; }
}

public class PostUpdateRequest
{
    [Required]
    [StringLength(2000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImageUrl { get; set; }
}

public class PostResponse
{
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
