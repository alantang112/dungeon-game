using System;
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

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            throw new NotImplementedException();
        }
    }
}
