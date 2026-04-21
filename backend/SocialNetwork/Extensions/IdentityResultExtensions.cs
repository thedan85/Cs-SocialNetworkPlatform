using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Extensions;

public static class IdentityResultExtensions
{
    public static IReadOnlyList<string> ToErrorMessages(this IdentityResult identityResult)
    {
        return identityResult.Errors
            .Select(error => error.Description)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToArray();
    }
}
