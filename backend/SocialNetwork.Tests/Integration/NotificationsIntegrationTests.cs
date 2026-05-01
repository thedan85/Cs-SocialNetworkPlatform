using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using Xunit;

namespace SocialNetwork.Tests.Integration;

public class NotificationsIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public NotificationsIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateNotification_ShouldReturnCreated_WhenUsersExist()
    {
        var senderId = "sender-1";
        var recipientId = "recipient-1";
        await SeedUserAsync(senderId);
        await SeedUserAsync(recipientId);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-test-user-id", senderId);

        var request = new NotificationCreateRequest
        {
            RecipientUserId = recipientId,
            Type = "Like",
            Content = "Sender liked your post."
        };

        var response = await client.PostAsJsonAsync("/api/notifications", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationResponse>>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.Equal(recipientId, body.Data!.RecipientUserId);
        Assert.Equal(senderId, body.Data.SenderUserId);
    }

    private async Task SeedUserAsync(string userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.Users.FindAsync(userId) != null)
        {
            return;
        }

        dbContext.Users.Add(new User
        {
            Id = userId,
            UserName = userId,
            Email = $"{userId}@example.com"
        });

        await dbContext.SaveChangesAsync();
    }
}
