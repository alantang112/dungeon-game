using System.Text.Json;
using System.Text.Json.Serialization;

namespace DungeonGame.Engine.Utilities
{
    public class SerializationUtility
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
    }
}