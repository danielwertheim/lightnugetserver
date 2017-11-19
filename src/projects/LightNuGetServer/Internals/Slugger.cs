using System.Text.RegularExpressions;

namespace LightNuGetServer.Internals
{
    internal static class Slugger
    {
        internal static string Slugify(this string value)
        {
            value = value.ToLowerInvariant();
            value = ReplaceInvalidChars(value);
            value = ReplaceMultiSpaces(value);
            value = ReplaceHyphens(value);

            return value;
        }

        private static string ReplaceInvalidChars(string value)
            => Regex.Replace(value, @"[^a-z0-9\s-]", string.Empty);

        private static string ReplaceMultiSpaces(string value)
            => Regex.Replace(value, @"\s+", " ").Trim();

        private static string ReplaceHyphens(string value)
            => Regex.Replace(value, @"\s", "-");
    }
}