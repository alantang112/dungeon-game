using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine
{
    public interface IGameEngine
    {
        event Action OnStateChanged;
        GameState CurrentState { get; }
        void ProcessInput(InputEvent inputEvent);
    }
}
