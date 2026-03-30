namespace SocialNetwork.Model;

public class UserFriendship
{
    public string FriendshipId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

    public virtual Friendship? Friendship { get; set; }

    public virtual User? User { get; set; }
}