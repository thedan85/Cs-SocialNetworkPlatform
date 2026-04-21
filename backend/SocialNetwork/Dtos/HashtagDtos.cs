using System.Collections.Generic;

namespace SocialNetwork.Dtos;

public class HashtagSearchResponse
{
    public string HashtagId { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public List<PostResponse> Posts { get; set; } = new();
}

public class HashtagTrendingResponse
{
    public string HashtagId { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}
