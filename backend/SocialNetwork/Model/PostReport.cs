namespace SocialNetwork.Model;

public class PostReport
{
    public string PostReportId { get; set; } = Guid.NewGuid().ToString();

    public string PostId { get; set; } = string.Empty;

    public string ReporterUserId { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public string? Description{get;set;}


    public Boolean Status{get;set;} = false; // false for pending, true for reviewed
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation properties
    public virtual Post? Post { get; set; }
    public virtual User? ReporterUser { get; set; }
}