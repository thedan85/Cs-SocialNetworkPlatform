using System.Text.RegularExpressions;

namespace SocialNetwork.Helpers;

public static class HashtagHelper
{
    private static readonly Regex HashtagRegex = new(@"(?<!\w)#([A-Za-z0-9_]{1,50})", RegexOptions.Compiled);

    public static IReadOnlyList<string> ExtractTags(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Array.Empty<string>();
        }

        var matches = HashtagRegex.Matches(content);
        if (matches.Count == 0)
        {
            return Array.Empty<string>();
        }

        var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in matches)
        {
            var tagValue = match.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(tagValue))
            {
                continue;
            }

            var tag = $"#{tagValue}".ToLowerInvariant();
            if (tag.Length <= 100)
            {
                tags.Add(tag);
            }
        }

        return tags.ToList();
    }
}
