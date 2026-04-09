namespace SocialNetwork.Model;

public class Hashtag
{
    public string HashtagId { get; set; } = Guid.NewGuid().ToString();
    
    public string Tag { get; set; } = string.Empty;
    
    public int UsageCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}
