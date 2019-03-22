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
        private static readonly JObject[] Empty = new JObject[0];

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

        public static JObject[] FindClosest(
            JArray array,
            string query,
            int threshold,
            int maxItems)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Empty;
            }

            Console.WriteLine("===========");
            return array
                .OfType<JObject>()
                .Select(obj => (obj, dist: MeasurePropsDistance(obj, query)))
                .OrderBy(x => x.dist)
                .Select(x =>
                {
                    Console.WriteLine($"Dist: [{x.obj["_label"]}] [{query}] = {x.dist}");
                    return x;
                })
                .Where(x => x.dist <= threshold && x.obj["_label"] != null)
                .OrderBy(x => x.dist)
                .Select(x => x.obj)
                .Take(maxItems)
                .ToArray();
        }

        public static JObject LookupObject(JArray array, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            foreach (var item in array)
            {
                if (item is JObject obj && MatchId(obj, query))
                {
                    return obj;
                }
            }

            return null;
        }

        private static int MeasurePropsDistance(JObject obj, string query)
        {
            query = query.ToUpperInvariant();
            var result = Math.Min(
                MeasureDistance(obj["_id"], query),
                MeasureDistance(obj["_label"], query));
            return result;
        }

        private static int MeasureDistance(JToken token, string query)
        {
            switch (token)
            {
                case JValue simpleValue:
                    return simpleValue.Value is string str
                        ? StringUtils.Distance(str.ToUpperInvariant(), query)
                        : int.MaxValue;
                case JArray arrayValue:
                    return arrayValue.Min(x => MeasureDistance(x, query));
                default:
                    return int.MaxValue;
            }
        }

        private static bool MatchId(JObject obj, string key)
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
    }
}
