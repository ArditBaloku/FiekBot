using System.Threading.Tasks;
using Discord.Commands;

namespace FiekBot.Modules
{
    [Name("Lëndët")]
    [Summary("Informata për lëndët")]
    public class SubjectsModule : Module
    {
        [Command("info")]
        [Summary("Merr informata rreth lëndës.")]
        public async Task SubjectInfo([Remainder, Name("lenda")] string subject)
        {
            await ReplyAsync($"Informatat për lëndën **{subject}**");
        }
    }
}
