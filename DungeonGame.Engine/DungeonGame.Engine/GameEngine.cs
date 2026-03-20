using System;
using System.Linq;
using System.Text.Json;
using DungeonGame.Engine.GameInputHandlers;
using DungeonGame.Engine.GameInputHandlers.Handlers;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine
{
    public class GameEngine : IGameEngine
    {
        private GameState CurrentState;

        public event Action OnStateChanged = delegate { };

        internal IGameInputHandler _gameInputHandler;

        private InputEventType[] InternalInputEventTypes = new InputEventType[] { InputEventType.MonstersMove, InputEventType.MonstersAttack };

        public GameEngine()
        {
            CurrentState = new GameState();
            _gameInputHandler = GetGameInputHandlers();
        }

        internal IGameInputHandler GetGameInputHandlers()
        {
            var firstHandler = new StartPhaseHandler();

            firstHandler
                .SetNext(new EnergyDicePreRollHandler())
                .SetNext(new EnergyDiceAssignmentHandler())
                .SetNext(new HeroActionsHandler())
                .SetNext(new MonsterActionsHandler());

            return firstHandler;
        }

        public GameState GetCurrentState() => CurrentState.DeepClone();

        public void ProcessInput(InputEvent inputEvent)
        {
            if (InternalInputEventTypes.Contains(inputEvent.EventType))
                throw new NotSupportedException($"Input Event Type not allowed: {inputEvent.EventType}");

            CurrentState.GameMessage = null;
            CurrentState.ScheduledEvents.Clear();

            CurrentState.ScheduledEvents.Add(inputEvent);

            while (CurrentState.ScheduledEvents.Any())
            {
                var scheduledEvent = CurrentState.ScheduledEvents[0];
                CurrentState.ScheduledEvents.RemoveAt(0);
                CurrentState = _gameInputHandler.Handle(CurrentState, scheduledEvent);
            }

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
