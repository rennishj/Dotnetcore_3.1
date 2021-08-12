using System;
using System.Net.Http;

namespace ConsoleClient
{
    public class MoviesClient
    {
        public MoviesClient(HttpClient client)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://localhost:44301");
            Client.Timeout = new TimeSpan(0, 0, 30);
            Client.DefaultRequestHeaders.Clear();
        }

        public HttpClient Client { get; set; }

    }
}
