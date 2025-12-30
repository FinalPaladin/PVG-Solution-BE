using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PVG.Domain.Utilities
{
    public static class StringHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            input = input.Trim().ToLowerInvariant();

            // remove dấu tiếng Việt
            input = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            input = sb.ToString().Normalize(NormalizationForm.FormC);

            // remove special chars
            input = Regex.Replace(input, @"[^a-z0-9\s-]", "");

            // replace spaces with hyphen
            input = Regex.Replace(input, @"\s+", "-");

            // trim hyphen
            return input.Trim('-');
        }

    }
}