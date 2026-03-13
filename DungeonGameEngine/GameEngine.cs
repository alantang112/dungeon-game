using System;
using DungeonGameEngine.GameInputHandlers;
using DungeonGameEngine.GameInputHandlers.Handlers;
using DungeonGameEngine.Models;
using DungeonGameEngine.Models.InputEventModels;

namespace DungeonGameEngine
{
    public class GameEngine : IGameEngine
    {
        public GameState CurrentState { get; private set; }

        public event Action OnStateChanged = delegate { };

        internal IGameInputHandler _gameInputHandler;

        public GameEngine()
        {
            CurrentState = new GameState();
            _gameInputHandler = GetGameInputHandlers();
        }

        public void ProcessInput(InputEvent inputEvent)
        {
            CurrentState = _gameInputHandler.Handle(CurrentState, inputEvent);

            // for now, assume that game state will always be modified after any input
            OnStateChanged?.Invoke();
        }

        internal IGameInputHandler GetGameInputHandlers()
        {
            return new StartPhaseHandler();
        }
    }
}
