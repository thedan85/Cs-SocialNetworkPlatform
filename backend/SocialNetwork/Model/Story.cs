namespace SocialNetwork.Model;

public class Story
{
    public string StoryId { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);
    
    // Navigation property
    public virtual User? User { get; set; }
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
