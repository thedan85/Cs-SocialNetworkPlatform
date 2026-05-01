using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using Xunit;

namespace SocialNetwork.Tests.Integration;

public class PostsIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public PostsIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePost_ShouldReturnCreated_WhenUserExists()
    {
        var userId = "user-1";
        await SeedUserAsync(userId);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("x-test-user-id", userId);

        var request = new PostCreateRequest
        {
            Content = "Hello from integration test"
        };

        var response = await client.PostAsJsonAsync("/api/posts", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PostResponse>>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.Equal("Hello from integration test", body.Data!.Content);
        Assert.Equal(userId, body.Data.UserId);
    }

    private async Task SeedUserAsync(string userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Users.Add(new User
        {
            Id = userId,
            UserName = userId,
            Email = $"{userId}@example.com"
        });

        await dbContext.SaveChangesAsync();
    }
}
