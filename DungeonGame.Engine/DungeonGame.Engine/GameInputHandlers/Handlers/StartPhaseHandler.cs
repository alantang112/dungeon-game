using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class StartPhaseHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.Start;
        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.NewGame };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            var (name, isMale) = CharacterNameUtility.GetRandomName();

            gameState.Hero.Name = $"Lil {name}";
            gameState.Hero.isMaleName = isMale;
            gameState.Hero.Stats.Add(SkillType.Movement, 1);
            gameState.Hero.Stats.Add(SkillType.Attack, 1);
            gameState.Hero.Stats.Add(SkillType.Defence, 1);
            gameState.Hero.Stats.Add(SkillType.AttackRange, 2);
            gameState.Hero.Health = GameConstants.HeroMaxHealth;
            gameState.Hero.BirthYear = DateTime.Now.Year;

            gameState.LevelNumber = 1;
            gameState.World = new World();
            gameState.World.InitializeLevel(gameState.LevelNumber!.Value);

            gameState.GamePhase = GamePhase.EnergyDicePreRoll;
            gameState.AddGameMessage(string.Format(GameMessages.YouHaveEnteredLevel, gameState.Hero.Name, gameState.LevelNumber));
            return gameState;
        }
    }  
}
