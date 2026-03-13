using System;
using DungeonGameEngine.Models;

namespace DungeonGameEngine
{
    public interface IGameEngine
    {
        event Action OnStateChanged;
        GameState CurrentState { get; }
        void ProcessInput(InputEvent inputEvent);
    }
}
