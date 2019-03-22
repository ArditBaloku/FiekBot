using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using FiekBot.Utils;
using Newtonsoft.Json.Linq;

namespace FiekBot.Modules
{
    [Name("Informata")]
    [Summary("Kërkim i informatave")]
    public class InfoModule : Module
    {
        private readonly JArray data;

        public InfoModule()
        {
            data = TryLoad("info.json");
        }

        [Command("info")]
        [Summary("Kërko informacion rreth një termi.")]
        public async Task SubjectInfo([Remainder, Name("termi")] string query)
        {
            var normalized = StringUtils.NormalizeQuery(query);
            if (normalized == null)
            {
                await ReplyAsync($"Termi **{query}** nuk u gjet.");
                return;
            }

            var obj = JsonUtils.LookupObject(data, normalized);
            if (obj != null)
            {
                await ReplyAsync($"Informatat për **{query}**", embed: JsonUtils.EmbedObject(obj));
                return;
            }

            const int threshold = 8;
            const int count = 3;

            // No match, try finding suggestions.
            var matches = JsonUtils.FindClosest(data, normalized, threshold, count);
            if (matches.Length != 0)
            {
                await ReplyAsync($"Termi **{query}** nuk u gjet.\n\n"
                                 + "Mos keni menduar për ndonjërën nga:\n"
                                 + matches.Select(match => "- " + match["_label"]).Join("\n"));
            }
            else
            {
                await ReplyAsync($"Termi **{query}** nuk u gjet.");
            }
        }

        private JArray TryLoad(string path)
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
    }
}
