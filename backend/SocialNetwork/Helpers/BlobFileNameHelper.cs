using System;
using System.IO;
using System.Linq;

namespace SocialNetwork.Helpers;

public static class BlobFileNameHelper
{
    public static string CreateUniqueBlobName(string originalFileName)
    {
        var safeFileName = Path.GetFileName(originalFileName);
        var extension = Path.GetExtension(safeFileName).ToLowerInvariant();
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeFileName);

        var normalized = new string(nameWithoutExtension
            .Select(character => char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : '-')
            .ToArray())
            .Trim('-');

        while (normalized.Contains("--", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("--", "-", StringComparison.Ordinal);
        }

        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = "file";
        }

        return $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}-{normalized}{extension}";
    }
}
