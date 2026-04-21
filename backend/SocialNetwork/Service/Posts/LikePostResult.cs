using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public class LikePostResult
{
    public LikeResponse Like { get; set; } = new();
    public bool IsCreated { get; set; }
}
