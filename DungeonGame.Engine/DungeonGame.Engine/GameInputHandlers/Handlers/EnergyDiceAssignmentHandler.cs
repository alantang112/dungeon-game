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
            new InputEventType[] { InputEventType.EnergyDiceAssign, InputEventType.EnergyDiceResetAssignment,InputEventType.EnergyDiceConfirmAssignment };

        private SkillType[] ValidSkillTypeForEnergyDiceAssignment = new SkillType[] { SkillType.Movement, SkillType.Attack, SkillType.Defence }; 

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.EnergyDiceAssign)
            {
                var parameters = (EnergyDiceAssignEventParameters) inputEvent.EventParameters!;

                if (!ValidSkillTypeForEnergyDiceAssignment.Contains(parameters.SkillType))
                {
                    gameState.GameMessage = GameConstants.InvalidSkillForEnergyDiceAssignment;
                    return gameState;
                }

                if (gameState.EnergyDice.AssignedSkills[parameters.DiceIndex] != null)
                {
                    gameState.GameMessage = GameConstants.SkillAlreadyAssignedEnergyDice;
                    return gameState;
                }

                gameState.EnergyDice.AssignDice(parameters.DiceIndex, parameters.SkillType);
                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.EnergyDiceConfirmAssignment)
            {
                if (gameState.EnergyDice.AssignedSkills.Any(x => x == null))
                {
                    gameState.GameMessage = GameConstants.AssignAllEnergyDiceBeforeProceeding;
                    return gameState;
                }

                gameState.GamePhase = GamePhase.HeroActions;
                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.EnergyDiceResetAssignment)
            {
                gameState.EnergyDice.ResetAssignment();
                return gameState;
            }

            throw new NotImplementedException();
        }
    }
}
