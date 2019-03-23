using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace FiekBot
{
    public class BotService : IDisposable
    {
        private readonly DiscordSocketClient discord;
        private readonly CommandService commands;
        private readonly IConfigurationRoot config;
        private readonly IServiceProvider serviceProvider;

        public BotService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider serviceProvider)
        {
            this.discord = discord;
            this.commands = commands;
            this.config = config;
            this.serviceProvider = serviceProvider;
        }

        public async Task RunAsync(CancellationToken cancellation)
        {
            var discordToken = config["tokens:discord"];
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                throw new InvalidOperationException("No discord token specified.");
            }

            discord.MessageReceived += OnMessageReceivedAsync;
            await discord.LoginAsync(TokenType.Bot, discordToken);
            await discord.StartAsync();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
            await Task.Delay(-1, cancellation);
            discord.MessageReceived -= OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            // Ensure the message is from a user/bot.
            if (!(s is SocketUserMessage msg))
            {
                return;
            }

            // Ignore self when checking commands.
            if (msg.Author.Id == discord.CurrentUser.Id)
            {
                return;
            }

            // Create the command context
            var context = new SocketCommandContext(discord, msg);

            // Check if the message has a valid command prefix.
            var argPos = 0;
            if (msg.HasStringPrefix(config["prefix"], ref argPos)
                || msg.HasMentionPrefix(discord.CurrentUser, ref argPos))
            {
                // Execute the command.
                var result = await commands.ExecuteAsync(context, argPos, serviceProvider);

                // If not successful, reply with the error.
                if (!result.IsSuccess)
                {
                    string errorMessage;
                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand:
                            errorMessage = "Komandë e panjohur.";
                            break;
                        case CommandError.ParseFailed:
                            errorMessage = "Format jo-valid i komandës.";
                            break;
                        case CommandError.BadArgCount:
                            errorMessage = "Numër jo-valid i argumenteve.";
                            break;
                        case CommandError.ObjectNotFound:
                        case CommandError.MultipleMatches:
                        case CommandError.UnmetPrecondition:
                        case CommandError.Exception:
                        case CommandError.Unsuccessful:
                            // TODO
                            errorMessage = result.ToString();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    await context.Channel.SendMessageAsync(errorMessage);
                }
            }
        }

        public void Dispose()
        {
            discord.MessageReceived -= OnMessageReceivedAsync;
        }
    }
}
