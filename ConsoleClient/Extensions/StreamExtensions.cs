using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

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

        public static void  SerializeToJsonAndWrite<T>(this Stream stream, T objectToWrite) where T : class
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new NotSupportedException("Can't write to a stream that is not writable");
            
            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonTextWriter, objectToWrite);
                    jsonTextWriter.Flush();
                }
            }
            
        }
    }
}
