using System;

namespace SocialNetwork.Dtos;

public class PostShareResponse
{
    public string PostShareId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
