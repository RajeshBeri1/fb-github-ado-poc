using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Lib.Common.Business
{
    /// <summary>
    /// JsonConverter
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Json
    {
        private static readonly JsonSerializer Serializer;
        private static readonly JsonSerializerSettings Settings = new();

        static Json()
        {
            JsonConfiguration.Configure(Settings);
            Serializer = JsonSerializer.Create(Settings);
        }

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        public static T? Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);

        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static T? Deserialize<T>(Stream stream)
        {
            using var reader = new StreamReader(stream, leaveOpen: true);
            using var jsonReader = new JsonTextReader(reader);
            return Serializer.Deserialize<T>(jsonReader);
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        public static string Serialize(object? @object) => JsonConvert.SerializeObject(@object, Settings);

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="stream">The stream.</param>
        public static void Serialize(object? @object, Stream stream)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(writer);
            Serializer.Serialize(jsonWriter, @object);
            jsonWriter.Flush();
        }
    }
}