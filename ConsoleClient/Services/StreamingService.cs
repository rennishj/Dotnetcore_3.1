using Newtonsoft.Json;
using System.IO;
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
            await GetPosterWithStream();
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

                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var poster = new JsonSerializer().Deserialize<Entity.Poster>(jsonTextReader);
                    }
                }
            }
        }
    }
}
