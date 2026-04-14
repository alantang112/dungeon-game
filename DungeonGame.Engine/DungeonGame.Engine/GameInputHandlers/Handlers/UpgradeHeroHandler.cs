using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class UpgradeHeroHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.UpgradeHero;
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.UpgradeHeroSetup, InputEventType.UpgradeHero };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.UpgradeHeroSetup)
            {
                if (gameState.World.UpgradePointsAvailable == 0)
                {
                    ContinueToEnergyDicePreRoll(gameState);
                    return gameState;
                }

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.UpgradeHero)
            {
                if (gameState.World.UpgradePointsAvailable == 0)
                    return gameState;

                var upgradeHeroParameters = inputEvent.EventParameters as UpgradeHeroEventParameters;
                if (upgradeHeroParameters == null)
                    throw new ArgumentNullException("UpgradeHeroEventParameters required for UpgradeHero input");

                if ((upgradeHeroParameters.SkillType.HasValue && upgradeHeroParameters.ReplenishHealth)
                    || (!upgradeHeroParameters.SkillType.HasValue && !upgradeHeroParameters.ReplenishHealth))
                {
                    return gameState;
                }

                if (upgradeHeroParameters.ReplenishHealth)
                {
                    gameState.Hero.Health = GameConstants.HeroMaxHealth;
                }    
                else
                {
                    gameState.Hero.Stats[upgradeHeroParameters.SkillType!.Value]++;
                }

                gameState.World.UpgradePointsAvailable--;

                if (gameState.World.UpgradePointsAvailable == 0)
                {
                    ContinueToEnergyDicePreRoll(gameState);
                }

                return gameState;
            }
            
            throw new NotImplementedException();
        }

        private void ContinueToEnergyDicePreRoll(GameState gameState)
        {
            gameState.GamePhase = GamePhase.EnergyDicePreRoll;
            gameState.ScheduledEvents.Add(new InputEvent()
            {
                EventType = InputEventType.EnergyDiceSetup
            });
        }
    }  
}
