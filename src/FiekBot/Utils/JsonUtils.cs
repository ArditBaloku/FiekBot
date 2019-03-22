using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord;
using Newtonsoft.Json.Linq;

namespace FiekBot.Utils
{
    internal static class JsonUtils
    {
        public static JObject LookupObject(JArray array, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            foreach (var item in array)
            {
                if (item is JObject obj && MatchId(obj, key))
                {
                    return obj;
                }
            }

            return null;
        }

        public static bool MatchId(JObject obj, string key)
        {
            return Match(obj["_id"], key);
        }

        private static bool Match(JToken token, string value)
        {
            switch (token)
            {
                case JValue simpleValue:
                    return simpleValue.Value is string str
                           && string.Equals(str.Replace(" ", ""), value, StringComparison.OrdinalIgnoreCase);
                case JArray arrayValue:
                    return arrayValue.Any(x => Match(x, value));
                default:
                    return false;
            }
        }

        public static Embed EmbedObject(JObject obj)
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(new Color(0xB69554));

            foreach (var pair in obj)
            {
                var key = pair.Key;
                var val = pair.Value;
                if (key.StartsWith("_"))
                {
                    continue;
                }

                var stringvalue = Stringify(val);
                if (!string.IsNullOrWhiteSpace(stringvalue))
                {
                    embedBuilder.AddField(key, stringvalue);
                }
            }

            return embedBuilder.Build();
        }

        private static string Stringify(JToken token)
        {
            switch (token)
            {
                case JArray array:
                    return array
                        .Select(Stringify)
                        .Where(v => v != null)
                        .Select(v => "- " + v)
                        .Join("\n");
                case JValue value:
                    return value.ToString(CultureInfo.InvariantCulture);
                default:
                    return null;
            }
        }

        private static string Join<T>(this IEnumerable<T> list, string separator)
        {
            return string.Join(separator, list);
        }
    }
}
