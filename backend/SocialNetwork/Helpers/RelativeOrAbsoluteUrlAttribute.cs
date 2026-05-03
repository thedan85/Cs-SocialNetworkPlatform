using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Helpers;

public sealed class RelativeOrAbsoluteUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string text)
        {
            return new ValidationResult(ErrorMessage ?? "Invalid URL.");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Success;
        }

        if (IsHttpUrl(text) || IsRelativePath(text))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(ErrorMessage ?? "Invalid URL.");
    }

    private static bool IsHttpUrl(string value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static bool IsRelativePath(string value)
    {
        var normalized = value.Replace("\\", "/");
        return normalized.StartsWith("/", StringComparison.Ordinal)
            || normalized.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase);
    }
}
