using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FiekBot.Math;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiekBot
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add current configuration to the collection.
            services.AddSingleton(Configuration);

            // Add discord to the collection.
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info.
                MessageCacheSize = 1000         // Cache 1,000 messages per channel.
            }));

            // Add the command service to the collection.
            services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info.
                DefaultRunMode = RunMode.Async, // Force all commands to run async by default.
            }));

            services.AddSingleton<IExpressionEvaluator, MathEvaluator>();

            // Add the bot service to the collection.
            services.AddSingleton<BotService>();
        }

        public Task Run(IServiceProvider serviceProvider)
        {
            var discordService = serviceProvider.GetRequiredService<BotService>();
            var cancellation = new CancellationTokenSource();
            return discordService.RunAsync(cancellation.Token);
        }
    }
}
