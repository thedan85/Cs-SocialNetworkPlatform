using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class LikeCreateRequest
{
    [Required]
    [StringLength(128)]
    public string UserId { get; set; } = string.Empty;
}

public class LikeResponse
{
    public string LikeId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? PostId { get; set; }
    public DateTime CreatedAt { get; set; }
}
