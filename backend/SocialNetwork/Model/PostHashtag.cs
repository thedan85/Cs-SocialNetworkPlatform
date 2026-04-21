namespace SocialNetwork.Model;

public class PostHashtag
{
    public string PostId { get; set; } = string.Empty;
    
    public string HashtagId { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Post? Post { get; set; }
    public virtual Hashtag? Hashtag { get; set; }
}
