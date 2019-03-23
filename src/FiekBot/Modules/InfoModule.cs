using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using FiekBot.Utils;
using Newtonsoft.Json.Linq;

namespace FiekBot.Modules
{
    [Name("Informata")]
    [Summary("Kërkim i informatave")]
    public class InfoModule : Module
    {
        private static readonly JArray Data;

        static InfoModule()
        {
            Data = TryLoad("info.json");
        }

        private static JArray TryLoad(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                return JArray.Parse(text);
            }
            catch
            {
                return new JArray();
            }
        }

        [Command("info")]
        [Summary("Kërko informacion rreth një termi.")]
        public async Task GetInfo([Remainder, Name("termi")] string query)
        {
            var normalized = StringUtils.NormalizeQuery(query);
            if (normalized == null)
            {
                await ReplyAsync($"Termi **{query}** nuk u gjet.");
                return;
            }

            var obj = JsonUtils.LookupObject(Data, normalized);
            if (obj != null)
            {
                await ReplyAsync(
                    $"Informatat për **{query}**",
                    embed: JsonUtils.EmbedObject(obj));
                return;
            }

            const int threshold = 8;
            const int count = 3;

            // No match, try finding suggestions.
            var matches = JsonUtils.FindClosest(Data, normalized, threshold, count);
            if (matches.Length != 0)
            {
                var emojis = new[] { "\u0031\u20E3", "\u0032\u20E3", "\u0033\u20E3" };

                // Give suggestions and listen for reactions.
                var text = $"Termi **{query}** nuk u gjet.\n\n"
                           + "Mos keni menduar për ndonjërën nga:\n"
                           + matches.Select((match, i) => i + 1 + ") " + match["_label"]).Join("\n");

                var callback = new ReactionCallbackData(
                    text,
                    embed: null,
                    expiresAfterUse: true,
                    singleUsePerUser: true,
                    timeout: TimeSpan.FromSeconds(30d));

                for (var i = 0; i < matches.Length; i++)
                {
                    var term = matches[i]["_label"].ToString();
                    callback.WithCallback(
                        new Emoji(emojis[i]), async (c, r) =>
                        {
                            var newObj = TryLookup(term);
                            if (newObj != null)
                            {
                                await c.Channel.SendMessageAsync(
                                    $"Informatat për **{term}**",
                                    embed: JsonUtils.EmbedObject(newObj));
                            }
                            else
                            {
                                await c.Channel.SendMessageAsync("Fatkeqësisht ka ndodhur një gabim. Ju lutem provoni përsëri.");
                            }
                        });
                }

                await InlineReactionReplyAsync(callback, fromSourceUser: true);
            }
            else
            {
                // No suggestions.
                await ReplyAsync($"Termi **{query}** nuk u gjet.");
            }
        }

        private JObject TryLookup(string query)
        {
            var normalized = StringUtils.NormalizeQuery(query);
            if (normalized == null)
            {
                return null;
            }

            return JsonUtils.LookupObject(Data, normalized);
        }
    }
}
