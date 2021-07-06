
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyCentral.Client;
using MyCentral.Client.Azure;
using System.Threading.Tasks;

namespace MyCentral.ConsoleTests
{

    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                  .ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.AddJsonFile("appsettings.json", optional: true);
                      config.AddEnvironmentVariables();

                      if (args != null)
                      {
                          config.AddCommandLine(args);
                      }
                  })
                  .ConfigureLogging((hostingContext, logging) =>
                  {
                      logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                  })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddOptions();
                    services.AddSingleton<TokenCredential, DefaultAzureCredential>();
                    services.AddOptions<IoTHubOptions>().Configure(options => hostContext.Configuration.GetSection("IoTHub").Bind(options));
                    services.AddSingleton<IServiceClient, AzureServiceClient>();
                    services.AddSingleton<IHostedService, IoTHubService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
