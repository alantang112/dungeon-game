using System.Runtime.Serialization;
using System.Text.Json;
using DungeonGame.Engine;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;
using Microsoft.JSInterop;

namespace DungeonGame.Wasm;

public static class JSBridge
{
    private static IGameEngine? _engine;

    [JSInvokable]
    public static GameState Initialize(IGameEngine? engine = null)
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

        return _engine.GetCurrentState();
    } 

    [JSInvokable]
    public static GameState ProcessInput(string inputEvent)
    {
        var inputEventModel = JsonSerializer.Deserialize<InputEvent>(inputEvent, SerializationUtility.JsonSerializerOptions);

        if (inputEventModel == null)
            throw new SerializationException($"Could not parsed inputEvent: {inputEvent}");

        var gameState = _engine!.ProcessInput(inputEventModel);
        return gameState;
    } 

    [JSInvokable]
    public static string GetGameStateSnapshot() => _engine!.GetGameStateSnapshot();
    
    [JSInvokable]
    public static GameState LoadGameStateSnapshot(string snapshot) => _engine!.LoadGameStateSnapshot(snapshot);
}
