using System.Text.Json;

namespace DungeonGame.Engine.Utilities
{
    public static class ObjectUtility
    {
        public static T DeepClone<T>(this T obj)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj))!;
        }
    }
}
