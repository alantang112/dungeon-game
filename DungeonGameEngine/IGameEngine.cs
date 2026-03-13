using System;
using DungeonGameEngine.Models;
using DungeonGameEngine.Models.InputEventModels;

namespace DungeonGameEngine
{
    public interface IGameEngine
    {
        event Action OnStateChanged;
        GameState CurrentState { get; }
        void ProcessInput(InputEvent inputEvent);
    }
}
