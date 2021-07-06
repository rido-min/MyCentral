using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCentral.Client;
using MyCentral.Client.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCentral.ConsoleTests
{
    class IoTHubService : IHostedService, IDisposable
    {
        private readonly ILogger<IoTHubService> _logger;
        private readonly IOptions<IoTHubOptions> _appConfig;
        private IServiceClient _client;

        public IoTHubService(ILogger<IoTHubService> logger, IOptions<IoTHubOptions> appConfig, IServiceClient client)
        {
            _logger = logger;
            _appConfig = appConfig;
            _client = client;
        }

       
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting " + _client.HostName);

            await foreach(var device in _client.GetDevicesAsync(cancellationToken))
            {
                _logger.LogInformation(device);
            }

            var resp = await _client.InvokeMethodAsync("client1", "comp1*reboot", "{ a: 1}");
            _logger.LogInformation(resp);
            //return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping.");

           

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}
