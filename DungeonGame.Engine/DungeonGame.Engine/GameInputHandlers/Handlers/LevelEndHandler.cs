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
            var nextLevelParameters = inputEvent.EventParameters as NextLevelEventParameters;
            if (nextLevelParameters == null)
                throw new ArgumentNullException("NextLevelEventParameters required for NextLevel input");

            if ((nextLevelParameters.SkillType.HasValue && nextLevelParameters.ReplenishHealth)
                || (!nextLevelParameters.SkillType.HasValue && !nextLevelParameters.ReplenishHealth))
            {
                gameState.AddGameMessage(GameMessages.LevelUpError);
                return gameState;
            }

            if (nextLevelParameters.ReplenishHealth)
            {
                gameState.Hero.Health = GameConstants.HeroMaxHealth;
                gameState.AddGameMessage(string.Format(GameMessages.LevelUpReplenishHealth, gameState.Hero.Name));
            }    
            else
            {
                gameState.Hero.Stats[nextLevelParameters.SkillType!.Value]++;
                gameState.AddGameMessage(string.Format(GameMessages.LevelUpSkill, gameState.Hero.Name, nextLevelParameters.SkillType!.Value));
            }

            gameState.LevelNumber++;
            gameState.World.InitializeLevel(gameState.LevelNumber!.Value);
            gameState.GamePhase = GamePhase.EnergyDicePreRoll;

            gameState.AddGameMessage(string.Format(GameMessages.YouHaveEnteredLevel, gameState.Hero.Name, gameState.LevelNumber));

            return gameState;
        }
    }  
}
