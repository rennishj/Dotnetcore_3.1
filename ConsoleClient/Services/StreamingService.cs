using Marvin.StreamExtensions;
using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient.Services
{
    public class StreamingService : IIntegrationService
    {

        // private static HttpClient _httpClient = new HttpClient();
        private static HttpClient _httpClient = new HttpClient
            (            
                new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                }
            );
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
            await GetPosterWithGZipCompression();

            // await SendingAndReadingDataUsingStreaming();

            // await PostPosterWithStream();
            //await TestGetPosterWithStreaming();

            //await TestGetPosterWithoutStreaming();

            //await TestGetPosterWithStreamAndCompletionMode();
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

        /// <summary>
        /// This one uses gzip compression format ( We have to see if the api supports it)
        /// </summary>
        /// <returns></returns>
        private async Task GetPosterWithGZipCompression()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/posters/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip")); //This ensures that we get a compressed response

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode(); //throws exception if something bad happens

            //dispose the streams onec done with them
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var poster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
            }
        }

        #region Poster

        private async Task PostPosterWithStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation
            {
                Bytes = generatedBytes,
                Name = $"A poster for Inglorious Bastards_{DateTime.UtcNow.Ticks}",
                MovieId = 4
            };

            //write the data into a stream
            var memoryStream = new MemoryStream();
            memoryStream.SerializeToJsonAndWrite(posterForCreation);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/posters?movieId=1"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var streamContent = new StreamContent(memoryStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();

                    var createdPoster = JsonConvert.DeserializeObject<Entity.Poster>(createdContent);

                    //do something with the created content


                }
            }

        }

        private async Task SendingAndReadingDataUsingStreaming()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation
            {
                Bytes = generatedBytes,
                Name = $"A poster for Inglorious Bastards_{DateTime.UtcNow.Ticks}",
                MovieId = 4
            };

            //write the data into a stream
            var memoryStream = new MemoryStream();
            memoryStream.SerializeToJsonAndWrite(posterForCreation, 
                new UTF8Encoding(),
                true               
                );
            memoryStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/posters?movieId=1"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var streamContent = new StreamContent(memoryStream))
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    request.Content = streamContent;

                    var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var createdPoster = stream.ReadAndDeserializeFromJson<Entity.Poster>();
                    }                    
                }
            }

        }

        #endregion


    }
}
