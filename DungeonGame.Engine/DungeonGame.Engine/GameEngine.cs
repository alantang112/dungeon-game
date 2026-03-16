using System;
using System.Text.Json;
using DungeonGame.Engine.GameInputHandlers;
using DungeonGame.Engine.GameInputHandlers.Handlers;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine
{
    public class GameEngine : IGameEngine
    {
        private GameState CurrentState;

        public event Action OnStateChanged = delegate { };

        internal IGameInputHandler _gameInputHandler;

        public GameEngine()
        {
            CurrentState = new GameState();
            _gameInputHandler = GetGameInputHandlers();
        }

        internal IGameInputHandler GetGameInputHandlers()
        {
            var firstHandler = new StartPhaseHandler();

            firstHandler.SetNext(new EnergyDicePreRollHandler());

            return firstHandler;
        }

        public GameState GetCurrentState()
        {
            // return deep copy so client cannot modify
            return JsonSerializer.Deserialize<GameState>(JsonSerializer.Serialize(CurrentState))!;
        }

        public void ProcessInput(InputEvent inputEvent)
        {
            CurrentState = _gameInputHandler.Handle(CurrentState, inputEvent);
            OnStateChanged?.Invoke();
        }

        public string GetGameStateSnapshot()
        {
            return JsonSerializer.Serialize(CurrentState);
        }

        public void LoadGameStateSnapshot(string snapshot)
        {
            var gameState = JsonSerializer.Deserialize<GameState>(snapshot);

            if (gameState == null)
                throw new ArgumentException("Invalid game state snapshot");

            CurrentState = gameState;
            OnStateChanged?.Invoke();
        }
    }
}
