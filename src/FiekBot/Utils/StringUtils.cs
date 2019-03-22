using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static int Distance(string value, string query)
        {
            var dist = 0;
            if (value.Length >= 4)
            {
                if (value.Contains(query))
                {

                }
                else
                {
                    dist += DamerauLevenshteinDistance(value, query);
                }
            }
            else
            {
                dist += DamerauLevenshteinDistance(value, query);
            }

            if (value.Length < 5)
            {
                dist += 2 * (5 - value.Length);
            }

            if (query.Length < 4)
            {
                dist += 4 - query.Length;
            }

            value = value.Where(char.IsLetterOrDigit).Distinct().MakeString();
            query = query.Where(char.IsLetterOrDigit).Distinct().MakeString();
            dist += value.Except(query).Count();
            dist += query.Except(value).Count();
            return dist;
        }

        private static int DamerauLevenshteinDistance(string a, string b)
        {
            var bounds = new { Height = a.Length + 1, Width = b.Length + 1 };

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
                    var cost = a[height - 1] == b[width - 1] ? 0 : 1;
                    var insertion = matrix[height, width - 1] + 1;
                    var deletion = matrix[height - 1, width] + 1;
                    var substitution = matrix[height - 1, width - 1] + cost;

                    var distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && a[height - 1] == b[width - 2] && a[height - 2] == b[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }

        public static string Join<T>(this IEnumerable<T> list, string separator)
        {
            return string.Join(separator, list);
        }

        public static string MakeString(this IEnumerable<char> chars)
        {
            return new string(chars.ToArray());
        }
    }
}
