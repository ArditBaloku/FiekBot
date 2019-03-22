using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FiekBot.Utils
{
    internal static class StringUtils
    {
        private static readonly Regex ReplacementRegex =
            new Regex(@"(?<=\s|^)(?:E|I|DHE|TE|NE)(?=\s|$)");

        private static readonly Regex WhitespaceRegex =
            new Regex(@"[\s\r\n-]");

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

        public static int GetDamerauLevenshteinDistance(string s, string t)
        {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

            var matrix = new int[bounds.Height, bounds.Width];

            for (var height = 0; height < bounds.Height; height++)
            {
                matrix[height, 0] = height;
            }

            for (var width = 0; width < bounds.Width; width++)
            {
                matrix[0, width] = width;
            }

            for (var height = 1; height < bounds.Height; height++)
            {
                for (var width = 1; width < bounds.Width; width++)
                {
                    var cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    var insertion = matrix[height, width - 1] + 1;
                    var deletion = matrix[height - 1, width] + 1;
                    var substitution = matrix[height - 1, width - 1] + cost;

                    var distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }
    }
}
