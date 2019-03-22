using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiekBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // App configuration: default -> config.json -> env -> args.
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["prefix"] = "!"
                })
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            var configuration = builder.Build();

            // Startup object that configures services.
            var startup = new Startup(configuration);

            // Build service collection and configure it.
            var services = new ServiceCollection();
            startup.ConfigureServices(services);

            // Run app using the built service provider.
            using (var serviceProvider = services.BuildServiceProvider())
            {
                await startup.Run(serviceProvider);
            }
        }
    }
}
