using System;
using System.Net;
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
                //only if you use proxy   - if inside a corporate network
                var clientHandler = new HttpClientHandler();
                var proxy = WebRequest.GetSystemWebProxy();
                proxy.Credentials = CredentialCache.DefaultCredentials;
                clientHandler.Proxy = proxy;
                using (var httpClient = new HttpClient(clientHandler))
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
