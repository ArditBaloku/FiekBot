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
    [Name("Magic 8ball")]
    [Summary("Pyet topin nje pyetje me pergjigje po ose jo")]
    public class EightBallModule : Module
    {
        private static readonly JArray Data;
        private static Random rand;

        static EightBallModule()
        {
            Data = TryLoad("8ball.json");
            rand = new Random();
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

        [Command("8ball")]
        [Summary("Pyet topin nje pyetje")]
        public async Task GetAnswer([Remainder, Name("pyetja")] string question = null)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                await ReplyAsync("Nuk ka pergjigje nese nuk ka pyetje");
                return;
            }

            string answer = Data[rand.Next(Data.Count)].ToString();

            await ReplyAsync(answer);
            return;

        }
    }
}
