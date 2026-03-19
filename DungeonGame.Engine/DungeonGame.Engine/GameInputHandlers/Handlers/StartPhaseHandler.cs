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

            gameState.Hero.Name = newGameParameters.HeroName;
            gameState.Hero.Stats.Add(SkillType.Movement, 1);
            gameState.Hero.Stats.Add(SkillType.Attack, 1);
            gameState.Hero.Stats.Add(SkillType.Defence, 1);
            gameState.Hero.Stats.Add(SkillType.AttackRange, 2);

            gameState.LevelNumber = 1;
            gameState.World = new World();
            gameState.World.InitializeLevel(1);

            gameState.GamePhase = GamePhase.EnergyDicePreRoll;
            return gameState;
        }
    }  
}
