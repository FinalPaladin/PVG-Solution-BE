using System.Text.RegularExpressions;

namespace PVG.Domain.Utilities
{
    public static class StringHelper
    {
        public static string GenerateSlug(string incomingString, string slugSeparator = "-")
        {
            return Regex.Replace(Regex.Replace(incomingString, "[^a-zA-Z0-9\\s]", string.Empty), "\\s+", slugSeparator).ToLower();
        }
    }
}