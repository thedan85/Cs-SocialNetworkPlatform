namespace SocialNetwork.Model;

public class Comment
{
    public string CommentId { get; set; } = Guid.NewGuid().ToString();
    
    public string PostId { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public int LikeCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Post? Post { get; set; }
    public virtual User? User { get; set; }
}
