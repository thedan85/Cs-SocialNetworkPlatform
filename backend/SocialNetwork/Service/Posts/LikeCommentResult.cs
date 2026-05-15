using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public class LikeCommentResult
{
    public LikeResponse Like { get; set; } = new();
    public bool IsCreated { get; set; }
}
