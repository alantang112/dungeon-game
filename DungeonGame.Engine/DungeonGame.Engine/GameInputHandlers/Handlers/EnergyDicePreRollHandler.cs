using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class EnergyDicePreRollHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.EnergyDicePreRoll;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.EnergyDiceRoll };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            

            gameState.GamePhase = GamePhase.EnergyDiceAssignment;
            return gameState;
        }
    }
}
