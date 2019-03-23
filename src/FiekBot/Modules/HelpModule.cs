using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace FiekBot.Modules
{
    [Name("Ndihma")]
    [Summary("Informata rreth komandave.")]
    public class HelpModule : Module
    {
        private readonly CommandService service;
        private readonly IConfigurationRoot config;

        public HelpModule(CommandService service, IConfigurationRoot config)
        {
            this.service = service;
            this.config = config;
        }

        [Command("ndihme"), Alias("help")]
        public async Task HelpAsync()
        {
            var prefix = config["prefix"];
            var builder = new EmbedBuilder
            {
                Color = Color.Teal,
                Description = "Komandat të cilat mund t'i përdorni"
            };

            foreach (var module in service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("ndihme"), Alias("help")]
        public async Task HelpAsync([Name("komanda")] string command)
        {
            var result = service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Komanda **{command}** nuk ekziston.");
                return;
            }

            var builder = new EmbedBuilder
            {
                Color = Color.Teal,
                Description = $"Informata për **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parametrat: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Përshkrimi: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
