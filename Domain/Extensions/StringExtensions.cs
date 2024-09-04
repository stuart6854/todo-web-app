using Ganss.Xss;

namespace Domain.Extensions;

public static class StringExtensions
{
    public static string Sanitize(this string str)
    {
        var sanitizedString = string.Empty;
        if (!string.IsNullOrWhiteSpace(str))
        {
            var sanitizer = new HtmlSanitizer();
            sanitizedString = sanitizer.Sanitize(str);
        }

        return sanitizedString;
    }
}