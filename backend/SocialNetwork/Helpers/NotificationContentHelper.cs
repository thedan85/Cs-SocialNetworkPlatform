namespace SocialNetwork.Helpers;

public static class NotificationContentHelper
{
    public static string BuildFriendRequestContent(string requesterUserName)
    {
        return $"{requesterUserName} sent you a friend request.";
    }

    public static string BuildFriendAcceptedContent(string accepterUserName)
    {
        return $"{accepterUserName} accepted your friend request.";
    }

    public static string BuildPostSharedContent(string sharerUserName, string postAuthorName)
    {
        if (string.IsNullOrWhiteSpace(postAuthorName))
        {
            return $"{sharerUserName} shared a post.";
        }

        return $"{sharerUserName} shared {postAuthorName}'s post.";
    }
}
