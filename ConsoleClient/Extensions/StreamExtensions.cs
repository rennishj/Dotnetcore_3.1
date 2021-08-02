using Newtonsoft.Json;
using System;
using System.IO;

namespace ConsoleClient.Extensions
{
    public static class StreamExtensions
    {
        public static T ReadAndDeserializeFromJson<T>(this Stream stream) where T : class
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new NotSupportedException("Can't read from a stream that is not readable");

            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return new JsonSerializer().Deserialize<T>(jsonTextReader);                    
                }
            }
        }
    }
}
