using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class LevelEndHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.LevelEnd;
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.NextLevel };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            gameState.LevelNumber++;
            gameState.InitializeLevel(gameState.LevelNumber!.Value);

            gameState.AddGameMessage(string.Format(GameMessages.YouHaveEnteredLevel, gameState.Hero.Name, gameState.LevelNumber));

            gameState.PostInitializeLevel();

            return gameState;
        }
    }  
}
