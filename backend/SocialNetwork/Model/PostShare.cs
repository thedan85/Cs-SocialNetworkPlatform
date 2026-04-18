namespace SocialNetwork.Model;

public class PostShare
{
    public string PostShareId { get; set; } = Guid.NewGuid().ToString();

    public string PostId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Post? Post { get; set; }
    public virtual User? User { get; set; }
}
