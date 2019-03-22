using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FiekBot.Utils
{
    internal static class StringUtils
    {
        private static readonly Regex ReplacementRegex = 
            new Regex(@"(?<=\s|^)(?:E|I|DHE|TE)(?=\s|$)");

        private static readonly Regex WhitespaceRegex =
            new Regex(@"[\s\r\n]");

        public static string NormalizeQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            query = query.Trim().WithoutDiacritics().ToUpperInvariant();
            query = ReplacementRegex.Replace(query, "");
            query = WhitespaceRegex.Replace(query, "");
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            return query;
        }

        public static string WithoutDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
