using System;
using DungeonGameEngine.Models;
using DungeonGameEngine.Models.InputEventModels;

namespace DungeonGameEngine.GameInputHandlers.Handlers
{
    internal class StartPhaseHandler : AbstactGameInputHandler
    {
        protected override GamePhase[] HandledGamePhases => new GamePhase[] { GamePhase.Start };
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.NewGame };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            var newGameParameters = inputEvent.EventParameters as NewGameEventParameters;
            if (newGameParameters == null)
                throw new ArgumentException("NewGameEventParameters required for NewGame input");

            var heroName = newGameParameters.HeroName; // TODO: use this for something

            gameState.GamePhase = GamePhase.EnergyDicePreRoll;
            return gameState;
        }
    }  
}
