using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Dtos;

public class FriendRequestCreateRequest
{
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
    public string? User1Name { get; set; }
    public string? User1FirstName { get; set; }
    public string? User1LastName { get; set; }
    public string? User2Name { get; set; }
    public string? User2FirstName { get; set; }
    public string? User2LastName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
