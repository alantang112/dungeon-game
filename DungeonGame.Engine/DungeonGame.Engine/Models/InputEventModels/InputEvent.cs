using System.Text.Json.Serialization;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    [JsonConverter(typeof(InputEventConverter))]
    public class InputEvent
    {
        public InputEventType EventType { get; set; }
        public IInputEventParameters? EventParameters { get; set;}
    }
}
