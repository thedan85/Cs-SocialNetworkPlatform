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
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio,
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
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            LikeCount = post.LikeCount,
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

    public static FriendshipResponse ToFriendshipResponse(this Friendship friendship)
    {
        return new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
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
