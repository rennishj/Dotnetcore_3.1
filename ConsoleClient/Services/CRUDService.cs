using Entity;
using Movies.Client.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleClient.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        public CRUDService()
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
            //await GetResource();
            // await GetResourceThroughHttpRequestMessage();

            //await CreateResource();

            //await UpdateResource();
            await DeleteResource();
            //await Task.CompletedTask;
        }

        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies/getallmovies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();
            if(response.Content.Headers.ContentType.MediaType == "application/json")
            {
                 movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            else if(response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var serializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
            }
            
        }


        /// <summary>
        /// This is the preferred method as you can manipulate the request headers for each request.
        /// If you have to integerate with different vendors that requires different  request headers, this is the way to go.
        /// </summary>
        /// <returns></returns>
        public async Task GetResourceThroughHttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/getallmovies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); //throws exception if something bad happens
            var content = await response.Content.ReadAsStringAsync();

            var movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        }

        public async Task CreateResource()
        {
            //CTRL + SHIFT + Space then up or down arrow to see method overlosa
            var movie = new MovieForCreation
            {
                Title = "Inglorious Bastards",
                Description = "The Quentin Tarrentino movie",
                Director = "Quentin Tarrentino",
                ReleaseDate = new System.DateTimeOffset(new System.DateTime(2010, 09, 03)),
                Genre = "Thriller"
            };

            var movieJson = JsonSerializer.Serialize(movie);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies/create");
            request.Content = new StringContent(movieJson);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var createdMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task UpdateResource()
        {
            var movie = new MovieForUpdate
            {
                Id = 5,
                Title = "The Departed",
                Description = "Departed Movie",
                Director = "Martin Scorsesee",
                ReleaseDate = new System.DateTimeOffset(new System.DateTime(2006, 09, 03)),
                Genre = "Crime"
            };

            var movieJson = JsonSerializer.Serialize(movie);
            var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5");
            request.Content = new StringContent(movieJson);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var updatedMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task DeleteResource()
        {            
            var request = new HttpRequestMessage(HttpMethod.Delete, "5");            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();            
        }
    }
}
