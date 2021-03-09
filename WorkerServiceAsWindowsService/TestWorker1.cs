using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Csob.Project.WindowsService
{
    public class TestWorker1 : BackgroundService
    {

        private readonly IServiceProvider _services;

        public TestWorker1(IServiceProvider services)
        {
            _services = services;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
              {
                  while (!stoppingToken.IsCancellationRequested)
                  {
                      using var scope = _services.CreateScope();
                      var logger = scope.ServiceProvider.GetRequiredService<ILogger<TestWorker1>>();
                      logger.LogInformation("TestWorker1: {time}", DateTimeOffset.Now);
                      await Task.Delay(2000, stoppingToken);
                  }
              }, stoppingToken);
        }
    }
}
