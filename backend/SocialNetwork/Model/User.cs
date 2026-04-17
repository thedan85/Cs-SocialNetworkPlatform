using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Model;

public class User : IdentityUser
{    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
    public ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();
    public ICollection<Friendship> FriendshipsAsUser1 { get; set; } = new List<Friendship>();
    public ICollection<Friendship> FriendshipsAsUser2 { get; set; } = new List<Friendship>();
    public ICollection<UserFriendship> UserFriendships { get; set; } = new List<UserFriendship>();
    public ICollection<PostReport> PostReports { get; set; } = new List<PostReport>();
}
