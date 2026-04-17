namespace SocialNetwork.Model;

public static class PostPrivacy
{
    public const string Public = "Public";
    public const string Friends = "Friends";
    public const string Private = "Private";

    public static readonly IReadOnlyList<string> Allowed = new[]
    {
        Public,
        Friends,
        Private
    };
}
