using System;
using System.Linq;
using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class EnergyDicePreRollHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.EnergyDicePreRoll;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.EnergyDiceSetup, InputEventType.EnergyDiceRoll };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.EnergyDiceSetup)
            {
                gameState.World.HeroActionPoints.Clear();
                gameState.EnergyDice.ResetAssignment();

                gameState.World.Monsters.ForEach(mp =>
                {
                    if (mp.Monster.IsBossType && !mp.Monster.BossDice.Any())
                    {
                        mp.Monster.BossDice.Add(SkillType.Movement, RandomUtility.RollDice(mp.Monster.BossDiceType!.Value));
                        mp.Monster.BossDice.Add(SkillType.Attack, RandomUtility.RollDice(mp.Monster.BossDiceType!.Value));
                        mp.Monster.BossDice.Add(SkillType.Defence, RandomUtility.RollDice(mp.Monster.BossDiceType!.Value));
                    }
                });

                MonsterSpecialFunctions.TurnStartMonsterFunctions(gameState);

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.EnergyDiceRoll)
            {
                gameState.EnergyDice.Roll();
                gameState.EnergyDice.ResetAssignment();
                gameState.GamePhase = GamePhase.EnergyDiceAssignment;
                gameState.AddGameMessage(string.Format(GameMessages.DiceRolled, gameState.EnergyDice.Dice[0], gameState.EnergyDice.Dice[1], gameState.EnergyDice.Dice[2]));

                gameState.SaveTurnSnapshot();

                return gameState;
            }
            
            throw new NotImplementedException();
        }
    }
}
