namespace Telebox.Extensions;

public static class StringExtensions
{
    public static string ToSafeFileName(this string str)
    {
        return Path.GetInvalidFileNameChars()
            .Aggregate(str, (current, c) => current.Replace(c, '-'))
            .Replace(" ", "-");
    }
}