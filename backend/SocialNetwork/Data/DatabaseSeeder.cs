using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Extensions;
using SocialNetwork.Model;

namespace SocialNetwork.Data;

public static class DatabaseSeeder
{
    private const string DemoPassword = "Demo1234!";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await dbContext.Database.MigrateAsync(ct);

        await IdentitySeeder.SeedRolesAsync(services);

        if (await dbContext.Users.AnyAsync(ct))
        {
            return;
        }

        var now = DateTime.UtcNow;

        var admin = await CreateUserAsync(
            userManager,
            userName: "admin",
            firstName: "Admin",
            lastName: "User",
            email: "admin@socialnetwork.local",
            bio: "Platform administrator.",
            profilePicture: "https://images.example.com/profiles/admin.png",
            roles: ["Admin", "User"],
            ct);

        var alice = await CreateUserAsync(
            userManager,
            userName: "alice",
            firstName: "Alice",
            lastName: "Nguyen",
            email: "alice@socialnetwork.local",
            bio: "Frontend developer and coffee enthusiast.",
            profilePicture: "https://images.example.com/profiles/alice.png",
            roles: ["User"],
            ct);

        var bob = await CreateUserAsync(
            userManager,
            userName: "bob",
            firstName: "Bob",
            lastName: "Carter",
            email: "bob@socialnetwork.local",
            bio: "Backend engineer shipping APIs.",
            profilePicture: "https://images.example.com/profiles/bob.png",
            roles: ["User"],
            ct);

        var charlie = await CreateUserAsync(
            userManager,
            userName: "charlie",
            firstName: "Charlie",
            lastName: "Diaz",
            email: "charlie@socialnetwork.local",
            bio: "Product designer sharing ideas.",
            profilePicture: "https://images.example.com/profiles/charlie.png",
            roles: ["User"],
            ct);

        var welcomeTag = new Hashtag
        {
            Tag = "#welcome",
            UsageCount = 2,
            CreatedAt = now.AddDays(-7)
        };

        var apiTag = new Hashtag
        {
            Tag = "#api",
            UsageCount = 1,
            CreatedAt = now.AddDays(-5)
        };

        var designTag = new Hashtag
        {
            Tag = "#design",
            UsageCount = 1,
            CreatedAt = now.AddDays(-4)
        };

        var alicePost = new Post
        {
            UserId = alice.Id,
            Content = "Welcome to SocialNetwork. This is my first post.",
            LikeCount = 1,
            CreatedAt = now.AddDays(-6),
            UpdatedAt = now.AddDays(-6)
        };

        var bobPost = new Post
        {
            UserId = bob.Id,
            Content = "Shipping a new API endpoint today.",
            ImageUrl = "https://images.example.com/posts/api-release.png",
            LikeCount = 1,
            CreatedAt = now.AddDays(-5),
            UpdatedAt = now.AddDays(-5)
        };

        var charliePost = new Post
        {
            UserId = charlie.Id,
            Content = "Looking for feedback on the new profile screen.",
            ImageUrl = "https://images.example.com/posts/profile-screen.png",
            LikeCount = 0,
            CreatedAt = now.AddDays(-3),
            UpdatedAt = now.AddDays(-3)
        };

        var aliceStory = new Story
        {
            UserId = alice.Id,
            Content = "Morning build is green.",
            ImageUrl = null,
            CreatedAt = now.AddHours(-18),
            ExpiresAt = now.AddHours(6)
        };

        var bobStory = new Story
        {
            UserId = bob.Id,
            Content = "API schema is ready for review.",
            ImageUrl = "https://images.example.com/stories/schema-review.png",
            CreatedAt = now.AddHours(-12),
            ExpiresAt = now.AddHours(12)
        };

        var aliceCommentOnBobPost = new Comment
        {
            PostId = bobPost.PostId,
            UserId = alice.Id,
            Content = "This looks solid. Nice work.",
            LikeCount = 1,
            CreatedAt = now.AddDays(-4).AddHours(-2),
            UpdatedAt = now.AddDays(-4).AddHours(-2)
        };

        var bobCommentOnAlicePost = new Comment
        {
            PostId = alicePost.PostId,
            UserId = bob.Id,
            Content = "Great way to kick things off.",
            LikeCount = 0,
            CreatedAt = now.AddDays(-5).AddHours(-3),
            UpdatedAt = now.AddDays(-5).AddHours(-3)
        };

        var adminCommentOnCharliePost = new Comment
        {
            PostId = charliePost.PostId,
            UserId = admin.Id,
            Content = "Please add a short description for the screen states.",
            LikeCount = 0,
            CreatedAt = now.AddDays(-2).AddHours(-4),
            UpdatedAt = now.AddDays(-2).AddHours(-4)
        };

        alicePost.PostHashtags.Add(new PostHashtag { HashtagId = welcomeTag.HashtagId, PostId = alicePost.PostId });
        bobPost.PostHashtags.Add(new PostHashtag { HashtagId = welcomeTag.HashtagId, PostId = bobPost.PostId });
        bobPost.PostHashtags.Add(new PostHashtag { HashtagId = apiTag.HashtagId, PostId = bobPost.PostId });
        charliePost.PostHashtags.Add(new PostHashtag { HashtagId = designTag.HashtagId, PostId = charliePost.PostId });

        var acceptedFriendship = new Friendship
        {
            UserId1 = alice.Id,
            UserId2 = bob.Id,
            Status = "Accepted",
            CreatedAt = now.AddDays(-10),
            UpdatedAt = now.AddDays(-9)
        };

        var pendingFriendship = new Friendship
        {
            UserId1 = charlie.Id,
            UserId2 = alice.Id,
            Status = "Pending",
            CreatedAt = now.AddDays(-2)
        };

        acceptedFriendship.UserFriendships.Add(new UserFriendship
        {
            FriendshipId = acceptedFriendship.FriendshipId,
            UserId = alice.Id,
            AcceptedAt = now.AddDays(-9)
        });

        acceptedFriendship.UserFriendships.Add(new UserFriendship
        {
            FriendshipId = acceptedFriendship.FriendshipId,
            UserId = bob.Id,
            AcceptedAt = now.AddDays(-9)
        });

        var notifications = new List<Notification>
        {
            new()
            {
                RecipientUserId = bob.Id,
                SenderUserId = alice.Id,
                Type = "Like",
                Content = "Alice liked your API post.",
                CreatedAt = now.AddDays(-5).AddHours(2),
                IsRead = false
            },
            new()
            {
                RecipientUserId = alice.Id,
                SenderUserId = bob.Id,
                Type = "Comment",
                Content = "Bob commented on your first post.",
                CreatedAt = now.AddDays(-4).AddHours(3),
                IsRead = false
            },
            new()
            {
                RecipientUserId = alice.Id,
                SenderUserId = charlie.Id,
                Type = "FriendRequest",
                Content = "Charlie sent you a friend request.",
                CreatedAt = now.AddDays(-2).AddHours(1),
                IsRead = true
            }
        };

        var postReport = new PostReport
        {
            PostId = bobPost.PostId,
            ReporterUserId = charlie.Id,
            Reason = "Spam",
            Description = "Looks promotional and off-topic.",
            Status = false,
            CreatedAt = now.AddDays(-1)
        };

        alicePost.LikeCount = 1;
        bobPost.LikeCount = 1;

        await dbContext.Hashtags.AddRangeAsync([welcomeTag, apiTag, designTag], ct);
        await dbContext.Posts.AddRangeAsync([alicePost, bobPost, charliePost], ct);
        await dbContext.Stories.AddRangeAsync([aliceStory, bobStory], ct);
        await dbContext.Comments.AddRangeAsync([aliceCommentOnBobPost, bobCommentOnAlicePost, adminCommentOnCharliePost], ct);
        await dbContext.Likes.AddRangeAsync(
            [
                new Like
                {
                    UserId = admin.Id,
                    PostId = alicePost.PostId,
                    CreatedAt = now.AddDays(-5).AddHours(1)
                },
                new Like
                {
                    UserId = alice.Id,
                    PostId = bobPost.PostId,
                    CreatedAt = now.AddDays(-4).AddHours(1)
                },
                new Like
                {
                    UserId = charlie.Id,
                    CommentId = aliceCommentOnBobPost.CommentId,
                    CreatedAt = now.AddDays(-3).AddHours(5)
                },
                new Like
                {
                    UserId = admin.Id,
                    StoryId = bobStory.StoryId,
                    CreatedAt = now.AddHours(-10)
                }
            ],
            ct);
        await dbContext.Friendships.AddRangeAsync([acceptedFriendship, pendingFriendship], ct);
        await dbContext.UserFriendships.AddRangeAsync(acceptedFriendship.UserFriendships, ct);
        await dbContext.Notifications.AddRangeAsync(notifications, ct);
        await dbContext.PostReports.AddAsync(postReport, ct);

        await dbContext.SaveChangesAsync(ct);
    }

    private static async Task<User> CreateUserAsync(
        UserManager<User> userManager,
        string userName,
        string firstName,
        string lastName,
        string email,
        string? bio,
        string? profilePicture,
        IEnumerable<string> roles,
        CancellationToken ct)
    {
        var user = new User
        {
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            EmailConfirmed = true,
            Bio = bio,
            ProfilePicture = profilePicture,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, DemoPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.ToErrorMessages());
            throw new InvalidOperationException($"Failed to create seed user '{userName}': {errors}");
        }

        foreach (var role in roles)
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.ToErrorMessages());
                throw new InvalidOperationException($"Failed to assign role '{role}' to seed user '{userName}': {errors}");
            }
        }

        return user;
    }
}