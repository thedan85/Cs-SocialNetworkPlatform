using System.Collections.Generic;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Service;

public interface IJwtTokenService
{
    TokenResponse CreateToken(User user, IList<string> roles);
}
