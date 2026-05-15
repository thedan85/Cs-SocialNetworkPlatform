using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class CommentCreateRequest
{
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Content { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }
}

public class CommentResponse
{
    public string CommentId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
