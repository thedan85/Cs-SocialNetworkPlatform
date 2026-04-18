namespace SocialNetwork.Model;

public class Post
{
    public string PostId { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }

    public string Privacy { get; set; } = PostPrivacy.Public;
    
    public int LikeCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<PostShare> Shares { get; set; } = new List<PostShare>();
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
    
    // Navigation property
    public virtual User? User { get; set; }
}

