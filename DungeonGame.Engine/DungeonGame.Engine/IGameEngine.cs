using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine
{
    public interface IGameEngine
    {
        GameState GetCurrentState();
        GameState ProcessInput(InputEvent inputEvent);
        string GetGameStateSnapshot();
        GameState LoadGameStateSnapshot(string snapshot);
    }
}
