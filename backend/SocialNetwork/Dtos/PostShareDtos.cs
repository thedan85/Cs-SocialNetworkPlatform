using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class PostShareResponse
{
    public string PostShareId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PostShareCreateRequest
{
    [StringLength(2000)]
    public string? Content { get; set; }

    [StringLength(20)]
    public string? Privacy { get; set; }
}
