using System.Runtime.Serialization;
using System.Text.Json;
using DungeonGame.Engine;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;
using Microsoft.JSInterop;

namespace DungeonGame.Wasm;

public static class JSBridge
{
    private static IGameEngine? _engine;

    [JSInvokable]
    public static string Initialize(IGameEngine? engine = null)
    {
        if (engine != null)
        {
            if (_engine != null)
                throw new NotSupportedException("Game engine already initialized");
            
            _engine = engine;
        }
        else
        {
            if (_engine == null)
                throw new NotSupportedException("Game engine not yet initialized");
        }

        var gameState = _engine.GetCurrentState();
        return JsonSerializer.Serialize(gameState, SerializationUtility.JsonSerializerOptions);
    } 

    [JSInvokable]
    public static string ProcessInput(string inputEvent)
    {
        var inputEventModel = JsonSerializer.Deserialize<InputEvent>(inputEvent, SerializationUtility.JsonSerializerOptions);

        if (inputEventModel == null)
            throw new SerializationException($"Could not parsed inputEvent: {inputEvent}");

        var gameState = _engine!.ProcessInput(inputEventModel);
        return JsonSerializer.Serialize(gameState, SerializationUtility.JsonSerializerOptions);
    } 

    [JSInvokable]
    public static string GetGameStateSnapshot()
    {
        var gameState = _engine!.GetGameStateSnapshot();
        return JsonSerializer.Serialize(gameState, SerializationUtility.JsonSerializerOptions);
    }
    
    [JSInvokable]
    public static string LoadGameStateSnapshot(string snapshot)
    {
        var newGameState = _engine!.LoadGameStateSnapshot(snapshot);
        return JsonSerializer.Serialize(newGameState, SerializationUtility.JsonSerializerOptions);
    } 
}
