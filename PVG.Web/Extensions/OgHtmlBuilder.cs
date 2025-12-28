namespace PVG.Web.Extensions
{
    public static class OgHtmlBuilder
    {
        public static string BuildNewsHtml(
            string title,
            string description,
            string image,
            string url)
        {
            title = SeoEscape(title);
            description = SeoEscape(description);

            return $"""
            <!DOCTYPE html>
            <html lang="vi">
            <head>
                <meta charset="utf-8" />
                <title>{title}</title>

                <meta property="og:type" content="article" />
                <meta property="og:title" content="{title}" />
                <meta property="og:description" content="{description}" />
                <meta property="og:image" content="{image}" />
                <meta property="og:url" content="{url}" />

                <meta name="twitter:card" content="summary_large_image" />
            </head>
            <body>
                <script>
                    window.location.replace("{url}");
                </script>
            </body>
            </html>
            """;
        }

        /// <summary>
        /// Escape để tránh XSS + gãy meta
        /// </summary>
        private static string SeoEscape(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Trim();
        }
    }
}