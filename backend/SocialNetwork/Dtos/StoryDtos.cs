using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class StoryCreateRequest
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    [Url]
    public string? ImageUrl { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

public class StoryResponse
{
    public string StoryId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
