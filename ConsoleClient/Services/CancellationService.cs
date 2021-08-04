using Marvin.StreamExtensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
        });

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public CancellationService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:44300");
            _httpClient.Timeout = new TimeSpan(0, 0, 2);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //_cancellationTokenSource.CancelAfter(1000);
            //await GetPosterAndCancel(_cancellationTokenSource.Token);
            await GetPosterAndHandleTimeout();
        }

        private async Task GetPosterAndCancel(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); //This ensures that we get a compressed response
            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode(); //throws exception if something bad happens
                                                        //dispose the streams onec done with them
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var poster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Operation was cancelled with the message: {ex.Message}");
            }                                                                            
            
               
        }

        private async Task GetPosterAndHandleTimeout()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); //This ensures that we get a compressed response
            try
            {
                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode(); //throws exception if something bad happens
                                                        //dispose the streams onec done with them
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var poster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Operation was cancelled with the message: {ex.Message}");
            }


        }
    }
}
