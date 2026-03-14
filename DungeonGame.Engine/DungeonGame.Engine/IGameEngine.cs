using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine
{
    public interface IGameEngine
    {
        event Action OnStateChanged;
        GameState GetCurrentState();
        void ProcessInput(InputEvent inputEvent);
        string GetGameStateSnapshot();
        void LoadGameStateSnapshot(string snapshot);
    }
}
