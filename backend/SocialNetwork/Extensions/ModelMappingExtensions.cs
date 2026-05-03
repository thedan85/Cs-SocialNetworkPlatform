using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Extensions;

public static class ModelMappingExtensions
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePicture = user.ProfilePicture ?? string.Empty,
            Bio = user.Bio ?? string.Empty,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }

    public static PostResponse ToPostResponse(this Post post)
    {
        return new PostResponse
        {
            PostId = post.PostId,
            UserId = post.UserId,
            UserName = post.User?.UserName,
            FirstName = post.User?.FirstName,
            LastName = post.User?.LastName,
            ProfilePicture = post.User?.ProfilePicture,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            Privacy = string.IsNullOrWhiteSpace(post.Privacy) ? PostPrivacy.Public : post.Privacy,
            SharedPostId = post.SharedPostId,
            SharedPost = post.SharedPost is null ? null : post.SharedPost.ToSharedPostPreview(),
            LikeCount = post.LikeCount,
            ShareCount = 0,
            IsLiked = false,
            IsShared = false,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public static SharedPostPreview ToSharedPostPreview(this Post post)
    {
        return new SharedPostPreview
        {
            PostId = post.PostId,
            UserId = post.UserId,
            UserName = post.User?.UserName,
            FirstName = post.User?.FirstName,
            LastName = post.User?.LastName,
            ProfilePicture = post.User?.ProfilePicture,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            Privacy = string.IsNullOrWhiteSpace(post.Privacy) ? PostPrivacy.Public : post.Privacy,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }

    public static CommentResponse ToCommentResponse(this Comment comment)
    {
        return new CommentResponse
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            UserName = comment.User?.UserName,
            FirstName = comment.User?.FirstName,
            LastName = comment.User?.LastName,
            Content = comment.Content,
            LikeCount = comment.LikeCount,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static LikeResponse ToLikeResponse(this Like like)
    {
        return new LikeResponse
        {
            LikeId = like.LikeId,
            UserId = like.UserId,
            PostId = like.PostId,
            CreatedAt = like.CreatedAt
        };
    }

    public static PostShareResponse ToPostShareResponse(this PostShare share)
    {
        return new PostShareResponse
        {
            PostShareId = share.PostShareId,
            PostId = share.PostId,
            UserId = share.UserId,
            CreatedAt = share.CreatedAt
        };
    }

    public static FriendshipResponse ToFriendshipResponse(this Friendship friendship)
    {
        return new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
            User1Name = friendship.User1?.UserName,
            User1FirstName = friendship.User1?.FirstName,
            User1LastName = friendship.User1?.LastName,
            User2Name = friendship.User2?.UserName,
            User2FirstName = friendship.User2?.FirstName,
            User2LastName = friendship.User2?.LastName,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt
        };
    }

    public static NotificationResponse ToNotificationResponse(this Notification notification)
    {
        return new NotificationResponse
        {
            NotificationId = notification.NotificationId,
            RecipientUserId = notification.RecipientUserId,
            SenderUserId = notification.SenderUserId,
            Type = notification.Type,
            Content = notification.Content,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };
    }

    public static StoryResponse ToStoryResponse(this Story story)
    {
        return new StoryResponse
        {
            StoryId = story.StoryId,
            UserId = story.UserId,
            UserName = story.User?.UserName,
            FirstName = story.User?.FirstName,
            LastName = story.User?.LastName,
            Content = story.Content,
            ImageUrl = story.ImageUrl,
            CreatedAt = story.CreatedAt,
            ExpiresAt = story.ExpiresAt
        };
    }

    public static PostReportResponse ToPostReportResponse(this PostReport postReport)
    {
        return new PostReportResponse
        {
            PostReportId = postReport.PostReportId,
            PostId = postReport.PostId,
            ReporterUserId = postReport.ReporterUserId,
            Reason = postReport.Reason,
            Description = postReport.Description,
            Status = postReport.Status,
            CreatedAt = postReport.CreatedAt
        };
    }
}
