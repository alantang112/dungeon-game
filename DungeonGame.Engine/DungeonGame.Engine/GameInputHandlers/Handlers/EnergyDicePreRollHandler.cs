using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class EnergyDicePreRollHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.EnergyDicePreRoll;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.EnergyDiceRoll };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            gameState.EnergyDice.Roll();
            gameState.EnergyDice.ResetAssignment();
            gameState.GamePhase = GamePhase.EnergyDiceAssignment;
            gameState.AddGameMessage(GameMessages.DiceRolled);
            return gameState;
        }
    }
}
