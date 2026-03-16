using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class StartPhaseHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.Start;
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
