namespace SocialNetwork.Model;

public class Like
{
    public string LikeId { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId { get; set; } = string.Empty;
    
    public string? PostId { get; set; }
    
    public string? CommentId { get; set; }
    
    public string? StoryId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
    public virtual Story? Story { get; set; }
}
