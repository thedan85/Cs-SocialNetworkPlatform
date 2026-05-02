using Microsoft.Extensions.Options;
using SocialNetwork.Model;
using SocialNetwork.Service;
using SocialNetwork.Settings;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateService()
    {
        var settings = new JwtSettings
        {
            SecretKey = "test-secret-key-1234567890-abcdef",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenMinutes = 60,
            SessionTokenDays = 7
        };

        return new JwtTokenService(Options.Create(settings));
    }

    [Fact]
    public void CreateToken_ShouldReturnBearerToken()
    {
        var service = CreateService();
        var user = new User { Id = "user-1", UserName = "user-1", Email = "user-1@example.com" };

        var result = service.CreateToken(user, new List<string> { "User", "Admin" });

        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal(2, result.Roles.Count);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void CreateSessionToken_ShouldReturnUserId()
    {
        var service = CreateService();
        var user = new User { Id = "user-1" };

        var session = service.CreateSessionToken(user);
        var userId = service.GetUserIdFromSessionToken(session.Token);

        Assert.False(string.IsNullOrWhiteSpace(session.Token));
        Assert.Equal("user-1", userId);
    }

    [Fact]
    public void GetUserIdFromSessionToken_ShouldReturnNull_WhenTokenNotSession()
    {
        var service = CreateService();
        var user = new User { Id = "user-1", UserName = "user-1", Email = "user-1@example.com" };

        var accessToken = service.CreateToken(user, new List<string>());
        var userId = service.GetUserIdFromSessionToken(accessToken.AccessToken);

        Assert.Null(userId);
        Assert.Null(service.GetUserIdFromSessionToken(""));
    }
}
