using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class FriendRequestCreateRequest
{
    [Required]
    [StringLength(128)]
    public string RequesterUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string AddresseeUserId { get; set; } = string.Empty;
}

public class FriendshipResponse
{
    public string FriendshipId { get; set; } = string.Empty;
    public string UserId1 { get; set; } = string.Empty;
    public string UserId2 { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
