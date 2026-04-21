namespace SocialNetwork.Helpers;

public static class LocalEnvironmentLoader
{
    private const string LocalEnvironmentFileName = ".env.local";

    public static void LoadFromDirectory(string? baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            return;
        }

        var directory = new DirectoryInfo(baseDirectory);
        while (directory != null)
        {
            var envFilePath = Path.Combine(directory.FullName, LocalEnvironmentFileName);
            if (File.Exists(envFilePath))
            {
                LoadFile(envFilePath);
                return;
            }

            directory = directory.Parent;
        }
    }

    private static void LoadFile(string envFilePath)
    {
        foreach (var rawLine in File.ReadAllLines(envFilePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith("export ", StringComparison.OrdinalIgnoreCase))
            {
                line = line["export ".Length..].TrimStart();
            }

            var equalsIndex = line.IndexOf('=');
            if (equalsIndex <= 0)
            {
                continue;
            }

            var key = line[..equalsIndex].Trim();
            var value = UnwrapQuotes(line[(equalsIndex + 1)..].Trim());

            if (string.IsNullOrWhiteSpace(key) || Environment.GetEnvironmentVariable(key) is not null)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static string UnwrapQuotes(string value)
    {
        if (value.Length >= 2)
        {
            if ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\''))
            {
                return value[1..^1];
            }
        }

        return value;
    }
}