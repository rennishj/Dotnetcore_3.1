using Marvin.StreamExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MoviesClient _moviesClient;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, MoviesClient moviesClient)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _moviesClient = moviesClient ?? throw new ArgumentNullException(nameof(moviesClient));
        }
        public async Task Run()
        {
            //await TestDisposeHttpClient(_cancellationTokenSource.Token);
            // await TestUsingHttpClientFactory(_cancellationTokenSource.Token);

            await GetMoviesWithNamedHttpClientFactory(_cancellationTokenSource.Token);
            await GetMoviesWithTypedHttpClientFactory(_cancellationTokenSource.Token);
        }

        private async Task TestDisposeHttpClient(CancellationToken cancellationToken)
        {
            string url = "https://www.google.com";          

            for (int i = 0; i < 50; i++)
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

                        Console.WriteLine($"Request completed with status code: {response.StatusCode} at {DateTime.Now}");
                    }
                }
            }
        }

        private async Task TestReuseHttpClient(CancellationToken cancellationToken)
        {
            string url = "https://www.google.com";            
            var clientHandler = new HttpClientHandler();
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            clientHandler.Proxy = proxy;
            var httpClient = new HttpClient(clientHandler);

            for (int i = 0; i < 50; i++)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"Request completed with status code: {response.StatusCode} at {DateTime.Now}");
                }
            }
        }

        private async Task TestUsingHttpClientFactory(CancellationToken cancellationToken)
        {
            string url = "https://www.google.com";
            var clientHandler = new HttpClientHandler();
            var proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = CredentialCache.DefaultCredentials;
            clientHandler.Proxy = proxy;
            var httpClient = _httpClientFactory.CreateClient();            

            for (int i = 0; i < 50; i++)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"Request completed with status code: {response.StatusCode} at {DateTime.Now}");
                }
            }
        }

        private async Task GetMoviesWithNamedHttpClientFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("RennishClient");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/getallmovies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Entity.Movie>>();
                
            }
        }

        private async Task GetMoviesWithTypedHttpClientFactory(CancellationToken cancellationToken)
        {            
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/getallmovies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            using (var response = await _moviesClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Entity.Movie>>();

            }
        }
    }
}
