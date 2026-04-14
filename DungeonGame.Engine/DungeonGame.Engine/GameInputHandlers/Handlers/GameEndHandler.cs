using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class GameEndHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.GameEnd;
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.RetryLevel };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.RetryLevel)
            {
                if (gameState.LevelRetriesAvailable <= 0)
                    return gameState;

                gameState.LoadLevelSnapshot();
                gameState.LevelRetriesAvailable--;

                gameState.GamePhase = GamePhase.EnergyDicePreRoll;
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                    EventType = InputEventType.EnergyDiceSetup
                });

                return gameState;
            }
            
            throw new NotImplementedException();
        }
    }  
}
