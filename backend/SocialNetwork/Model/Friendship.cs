namespace SocialNetwork.Model;

public class Friendship
{
    public string FriendshipId { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId1 { get; set; } = string.Empty;
    
    public string UserId2 { get; set; } = string.Empty;

    public string Status { get; set;} = "Pending"; // Possible values: Pending, Accepted, Rejected
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User? User1 { get; set; }
    
    public virtual User? User2 { get; set; }
}