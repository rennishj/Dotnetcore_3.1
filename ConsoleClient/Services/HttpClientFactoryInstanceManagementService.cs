using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public async Task Run()
        {
            await TestDisposeHttpClient(_cancellationTokenSource.Token);
        }

        private async Task TestDisposeHttpClient(CancellationToken cancellationToken)
        {
            string url = "https://google.com";

            for (int i = 0; i < 10; i++)
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine($"Request completed with status code: {response.StatusCode}");
                    }
                }
            }
        }
    }
}
