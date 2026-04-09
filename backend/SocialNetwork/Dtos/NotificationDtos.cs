using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class NotificationCreateRequest
{
    [Required]
    [StringLength(128)]
    public string RecipientUserId { get; set; } = string.Empty;

    [StringLength(128)]
    public string SenderUserId { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(1000)]
    public string? Content { get; set; }
}

public class NotificationResponse
{
    public string NotificationId { get; set; } = string.Empty;
    public string RecipientUserId { get; set; } = string.Empty;
    public string SenderUserId { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
