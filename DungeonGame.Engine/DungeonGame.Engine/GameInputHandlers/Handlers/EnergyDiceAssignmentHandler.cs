using System;
using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

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
                    gameState.AddGameMessage(GameMessages.InvalidSkillForEnergyDiceAssignment);
                    return gameState;
                }

                if (gameState.EnergyDice.AssignedSkills[parameters.DiceIndex] != null)
                {
                    gameState.AddGameMessage(GameMessages.SkillAlreadyAssignedEnergyDice);
                    return gameState;
                }

                gameState.EnergyDice.AssignDice(parameters.DiceIndex, parameters.SkillType);
                gameState.AddGameMessage(string.Format(GameMessages.DiceAssignedToSkill, parameters.DiceIndex + 1, parameters.SkillType.ToString()));
                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.EnergyDiceResetAssignment)
            {
                gameState.EnergyDice.ResetAssignment();
                gameState.AddGameMessage(GameMessages.DiceAssignmentReset);
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.EnergyDiceConfirmAssignment)
            {
                if (gameState.EnergyDice.AssignedSkills.Any(x => x == null))
                {
                    gameState.AddGameMessage(GameMessages.AssignAllEnergyDiceBeforeProceeding);
                    return gameState;
                }

                gameState.World.HeroActionPoints.Clear();
                for (var i = 0; i < gameState.EnergyDice.AssignedSkills.Count(); i++)
                {
                    var skillType = (SkillType) gameState.EnergyDice.AssignedSkills[i];
                    var diceValue = gameState.EnergyDice.Dice[i];

                    gameState.World.HeroActionPoints.Add(skillType, gameState.Hero.Stats[skillType] + diceValue);
                }

                gameState.AddGameMessage(GameMessages.DiceAssignmentConfirmed);
                gameState.WorldSnapshot = gameState.World.DeepClone();
                gameState.GamePhase = GamePhase.HeroActions;
                return gameState;
            } 

            throw new NotImplementedException();
        }
    }
}
