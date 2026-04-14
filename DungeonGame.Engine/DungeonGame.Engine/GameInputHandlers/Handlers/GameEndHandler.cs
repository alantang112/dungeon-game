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
            if (inputEvent.EventType == InputEventType.RetryLevel) // TODO unit test
            {
                if (gameState.LevelRetriesAvailable <= 0)
                    return gameState;

                gameState.LoadLevelSnapshot();
                gameState.LevelRetriesAvailable--;

                gameState.PostInitializeLevel();

                return gameState;
            }
            
            throw new NotImplementedException();
        }
    }  
}
