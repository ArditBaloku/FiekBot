using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using FiekBot.Utils;
using Newtonsoft.Json.Linq;

namespace FiekBot.Modules
{
    [Name("Lëndët")]
    [Summary("Informata për lëndët")]
    public class SubjectsModule : Module
    {
        private readonly JArray subjects;

        public SubjectsModule()
        {
            subjects = TryLoad("info.json");
        }

        [Command("info")]
        [Summary("Merr informata rreth lëndës.")]
        public async Task SubjectInfo([Remainder, Name("lenda")] string subject)
        {
            var normalized = StringUtils.NormalizeQuery(subject);
            if (normalized == null)
            {
                await ReplyAsync("Jepni një emër valid të lëndës.");
                return;
            }

            var obj = JsonUtils.LookupObject(subjects, normalized);
            if (obj != null)
            {
                await ReplyAsync($"Informatat për lëndën **{subject}**", embed: JsonUtils.EmbedObject(obj));
            }
            else
            {
                await ReplyAsync($"Lënda **{subject}** nuk u gjet.");
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
