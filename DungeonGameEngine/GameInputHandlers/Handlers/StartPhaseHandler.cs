using DungeonGameEngine.Models;

namespace DungeonGameEngine.GameInputHandlers.Handlers
{
    internal class StartPhaseHandler : AbstactGameInputHandler
    {
        protected override GamePhase[] HandledGamePhases => new GamePhase[] { GamePhase.Start };
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.NewGame };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            gameState.GamePhase = GamePhase.EnergyDicePreRoll;
            return gameState;
        }
    }  
}


