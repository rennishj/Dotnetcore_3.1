using ConsoleClient.Extensions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleClient.Services
{
    public class StreamingService : IIntegrationService
    {

        private static HttpClient _httpClient = new HttpClient();
        public StreamingService()
        {
            _httpClient.BaseAddress = new System.Uri("https://localhost:44300/");
            _httpClient.Timeout = new System.TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            //default quality value is 1.0, so sever returns json instead of xml in this case
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
        }
        public async Task Run()
        {
            await TestGetPosterWithStreaming();

            await TestGetPosterWithoutStreaming();

            await TestGetPosterWithStreamAndCompletionMode();
        }

        private async Task TestGetPosterWithoutStreaming()
        { //warm up request
            await GetPosterWithoutStream();

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithoutStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed Time for TestGetPosterWithoutStreaming in milliseconds: {stopWatch.ElapsedMilliseconds}, Averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");

        }

        private async Task TestGetPosterWithStreaming()
        {
            //warm up request
            await GetPosterWithStream();

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed Time for GetPosterWithStream in milliseconds: {stopWatch.ElapsedMilliseconds}, Averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");

        }

        private async Task TestGetPosterWithStreamAndCompletionMode()
        {
            //warm up request
            await GetPosterWithStreamAndCompletionMode();

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStreamAndCompletionMode();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed Time for GetPosterWithStreamAndCompletionMode in milliseconds: {stopWatch.ElapsedMilliseconds}, Averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");

        }


        private async Task GetPosterWithoutStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); //throws exception if something bad happens

            var content = await response.Content.ReadAsStringAsync();
            var poster = JsonConvert.DeserializeObject<Entity.Poster>(content);
        }

        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); //throws exception if something bad happens

            //dispose the streams onec done with them
            using (var stream = await response.Content.ReadAsStreamAsync())
            {

                var poster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
            }
        }

        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode(); //throws exception if something bad happens

            //dispose the streams onec done with them
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var poster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
            }
        }

        
    }
}
