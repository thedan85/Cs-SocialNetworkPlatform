using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public class SharePostResult
{
    public PostShareResponse Share { get; set; } = new();
    public bool IsCreated { get; set; }
}
