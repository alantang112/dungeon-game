using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    public class InputEventConverter : JsonConverter<InputEvent>
    {
        public override InputEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 1. Parse the whole thing into a JsonDocument to "peek" at values
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        var result = new InputEvent();

        // 2. Get the EventType (Top Level)
        if (root.TryGetProperty(nameof(InputEvent.EventType), out var typeProp))
        {
            // Use the existing options (which should have the StringEnumConverter)
            result.EventType = typeProp.Deserialize<InputEventType>(options);
        }

        // 3. Get the EventParameters based on the Type we just found
        if (root.TryGetProperty(nameof(InputEvent.EventParameters), out var paramsProp))
        {
            result.EventParameters = result.EventType switch
            {
                InputEventType.EnergyDiceAssign => paramsProp.Deserialize<EnergyDiceAssignEventParameters>(options),
                InputEventType.HeroActionMove => paramsProp.Deserialize<HeroActionMoveEventParameters>(options),
                InputEventType.HeroActionAttack => paramsProp.Deserialize<HeroActionAttackEventParameters>(options),
                InputEventType.NextLevel => paramsProp.Deserialize<NextLevelEventParameters>(options),
                // Add other mappings here...
                _ => null
            };
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, InputEvent value, JsonSerializerOptions options)
    {
        // Standard serialization logic
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
    }
}
