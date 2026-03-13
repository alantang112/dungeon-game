using System;
using DungeonGame.Engine.GameInputHandlers;
using DungeonGame.Engine.GameInputHandlers.Handlers;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine
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
