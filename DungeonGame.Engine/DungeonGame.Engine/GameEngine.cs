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

        private InputEventType[] InternalInputEventTypes = new InputEventType[] { InputEventType.EnergyDiceSetup, InputEventType.MonstersMove, InputEventType.MonstersAttack, InputEventType.UpgradeHeroSetup };

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
                .SetNext(new MonsterActionsHandler())
                .SetNext(new LevelEndHandler())
                .SetNext(new GameEndHandler())
                .SetNext(new UpgradeHeroHandler())
                ;

            return firstHandler;
        }

        public GameState GetCurrentState()
        {
            var gameState = CurrentState.DeepClone();
            
            // Refresh ViewData whenever we are returning game state
            if (gameState.GamePhase == GamePhase.HeroActions)
            {
                gameState.ViewData.HeroCanWalkPositions = gameState.World.CalculateHeroCanWalkPositions();
                gameState.ViewData.HeroCanAttackPositions = gameState.World.CalculateHeroCanAttackPositions(gameState.Hero.Stats[SkillType.AttackRange]);
            }

            return gameState;
        }

        public GameState ProcessInput(InputEvent inputEvent)
        {
            if (InternalInputEventTypes.Contains(inputEvent.EventType))
                throw new NotSupportedException($"Input Event Type not allowed: {inputEvent.EventType}");

            // clean-up
            CurrentState.ViewData = new ViewData();
            CurrentState.ScheduledEvents.Clear();

            CurrentState.ScheduledEvents.Add(inputEvent);

            while (CurrentState.ScheduledEvents.Any())
            {
                var scheduledEvent = CurrentState.ScheduledEvents[0];
                CurrentState.ScheduledEvents.RemoveAt(0);
                CurrentState = _gameInputHandler.Handle(CurrentState, scheduledEvent);
            }

            return GetCurrentState();
        }

        public string GetGameStateSnapshot()
        {
            return JsonSerializer.Serialize(CurrentState, SerializationUtility.JsonSerializerOptions);
        }

        public GameState LoadGameStateSnapshot(string snapshot)
        {
            var gameState = JsonSerializer.Deserialize<GameState>(snapshot, SerializationUtility.JsonSerializerOptions);

            if (gameState == null)
                throw new ArgumentException("Invalid game state snapshot");

            CurrentState = gameState;
            return GetCurrentState();
        }
    }
}
