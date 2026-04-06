namespace SocialNetwork.Model;

public class Notification
{
    public string NotificationId {get; set;} = Guid.NewGuid().ToString();

    public string RecipientUserId {get; set;} = string.Empty;

    public string SenderUserId {get; set;} = string.Empty;

    public string Type {get; set;}

    public string Content {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public Boolean Isread {get; set;} = false;
}