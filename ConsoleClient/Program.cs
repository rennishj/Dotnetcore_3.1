using ConsoleClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host  = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Host created");
            try
            {
                
                await serviceProvider.GetService<IIntegrationService>().Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                   "An exception happened while running the integration service."); throw;
            }
            Console.ReadKey();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices(serviceCollection => ConfigureServices(serviceCollection));
        }

        private static void ConfigureServices(IServiceCollection servicesCollection)
        {
            servicesCollection.AddLogging(cfg => cfg.AddDebug().AddConsole());

            //servicesCollection.AddScoped<IIntegrationService, CRUDService>();
            // servicesCollection.AddScoped<IIntegrationService, StreamingService>();

            servicesCollection.AddScoped<IIntegrationService, CancellationService>();
        }
    }
}
