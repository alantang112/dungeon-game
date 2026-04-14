using System;
using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class EnergyDiceAssignmentHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.EnergyDiceAssignment;

        protected override InputEventType[] HandledInputEventTypes => 
            new InputEventType[] { InputEventType.EnergyDiceAssign, InputEventType.EnergyDiceResetAssignment, InputEventType.EnergyDiceReroll };

        private SkillType[] ValidSkillTypeForEnergyDiceAssignment = new SkillType[] { SkillType.Movement, SkillType.Attack, SkillType.Defence }; 

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.EnergyDiceAssign)
            {
                var parameters = (EnergyDiceAssignEventParameters) inputEvent.EventParameters!;

                if (!ValidSkillTypeForEnergyDiceAssignment.Contains(parameters.SkillType))
                {
                    return gameState;
                }

                if (gameState.EnergyDice.AssignedSkills[parameters.DiceIndex] != null)
                {
                    return gameState;
                }

                gameState.EnergyDice.AssignDice(parameters.DiceIndex, parameters.SkillType);

                // if all dice assigned, proceed to HeroActions
                if (gameState.EnergyDice.AssignedSkills.All(x => x != null))
                {
                    SetActionPointsAndProceedToHeroActions(gameState);
                }

                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.EnergyDiceResetAssignment)
            {
                gameState.EnergyDice.ResetAssignment();
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.EnergyDiceReroll)
            {
                if (gameState.World.RerollsAvailable <= 0)
                {
                    return gameState;
                }

                gameState.World.RerollsAvailable--;
                
                gameState.GamePhase = GamePhase.EnergyDicePreRoll;
                
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                    EventType = InputEventType.EnergyDiceSetup
                });

                return gameState;
            }

            throw new NotImplementedException();
        }

        private static void SetActionPointsAndProceedToHeroActions(GameState gameState)
        {
            gameState.World.HeroActionPoints.Clear();

            for (var i = 0; i < gameState.EnergyDice.AssignedSkills.Count(); i++)
            {
                var skillType = (SkillType) gameState.EnergyDice.AssignedSkills[i];
                var diceValue = gameState.EnergyDice.Dice[i];

                gameState.World.HeroActionPoints.Add(skillType, gameState.Hero.Stats[skillType] + diceValue);
            }

            gameState.GamePhase = GamePhase.HeroActions;   
        }
    }
}
