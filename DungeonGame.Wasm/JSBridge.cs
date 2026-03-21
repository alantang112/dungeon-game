using DungeonGame.Engine;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;
using Microsoft.JSInterop;

namespace DungeonGame.Wasm;

public static class JSBridge
{
    private static IGameEngine _engine;

    [JSInvokable]
    public static GameState Initialize(IGameEngine engine)
    {
        _engine = engine;
        return _engine.GetCurrentState();
    } 

    [JSInvokable]
    public static GameState ProcessInput(InputEvent inputEvent) => _engine.ProcessInput(inputEvent);

    [JSInvokable]
    public static string GetGameStateSnapshot() => _engine.GetGameStateSnapshot();
    
    [JSInvokable]
    public static GameState LoadGameStateSnapshot(string snapshot) => _engine.LoadGameStateSnapshot(snapshot);
}
