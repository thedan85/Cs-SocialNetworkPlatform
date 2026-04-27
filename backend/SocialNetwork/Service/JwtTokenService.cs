using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using SocialNetwork.Settings;

namespace SocialNetwork.Service;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public TokenResponse CreateToken(User user, IList<string> roles)
    {
        var key = GetSigningKey();
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            TokenType = "Bearer",
            Roles = roles.ToList()
        };
    }

    public SessionTokenResponse CreateSessionToken(User user)
    {
        var key = GetSigningKey();
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.SessionTokenDays);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("token_use", "session")
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new SessionTokenResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    public string? GetUserIdFromSessionToken(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(sessionToken, BuildValidationParameters(), out _);
            var tokenUse = principal.FindFirst("token_use")?.Value;
            if (!string.Equals(tokenUse, "session", StringComparison.Ordinal))
            {
                return null;
            }

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        catch
        {
            return null;
        }
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    }

    private TokenValidationParameters BuildValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = GetSigningKey(),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
}
