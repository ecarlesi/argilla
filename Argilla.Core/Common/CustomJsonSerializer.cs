using System.Text.Json;

namespace Argilla.Core.Common
{
    /// <summary>
    /// This class use the System.Text.Json.JsonSerializer class to serialize/deserialize the objects.
    /// </summary>
    public class CustomJsonSerializer
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Deserialize the json as argument and return the new instance.
        /// </summary>
        /// <typeparam name="T">Type of the created instance.</typeparam>
        /// <param name="json">The string contain the json to deserialize.</param>
        /// <returns>The deserialized instance.</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// Serialize the object as argument.
        /// </summary>
        /// <param name="o">The object to serialize.</param>
        /// <returns>The Json string.</returns>
        public static string Serialize(object o)
        {
            return JsonSerializer.Serialize(o, options);
        }
    }
}
