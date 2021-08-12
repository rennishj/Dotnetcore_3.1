using ConsoleClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
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

            //Named HttpClient            
            servicesCollection.AddHttpClient("RennishClient", client =>
            {                
                client.BaseAddress = new Uri("https://localhost:44300");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                };
            });

            /*
            //Typed Http client            
            servicesCollection.AddHttpClient<MoviesClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44301");
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                };
            });
            */

            // Moving the default HttpClient configuration to the typed class
            // We want to leave the ConfigurePrimaryHttpMessageHandler here to take advantage of the pooled HttpMessageHandler that the 
            // HttpClientFactory is using
            servicesCollection.AddHttpClient<MoviesClient>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                };
            });

            servicesCollection.AddScoped<IIntegrationService, HttpClientFactoryInstanceManagementService>();
            //servicesCollection.AddScoped<IIntegrationService, CRUDService>();
            // servicesCollection.AddScoped<IIntegrationService, StreamingService>();

            //servicesCollection.AddScoped<IIntegrationService, CancellationService>();
        }
    }
}
