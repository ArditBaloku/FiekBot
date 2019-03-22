using System.IO;
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
                await ReplyAsync("Jepni një emër valid të lëndës.");
                return;
            }

            var obj = JsonUtils.LookupObject(data, normalized);
            if (obj != null)
            {
                await ReplyAsync($"Informatat për **{query}**", embed: JsonUtils.EmbedObject(obj));
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
