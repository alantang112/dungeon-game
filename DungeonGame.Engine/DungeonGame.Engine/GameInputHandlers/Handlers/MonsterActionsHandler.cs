using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class MonsterActionsHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.MonsterActions;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.MonstersMove, InputEventType.MonstersAttack, InputEventType.MonsterActionsEnd };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.MonstersMove)
            {
                // TODO: PlotValuesByFloodSearch(
                //  1. Seed position with value
                //  2. Blockers
                //  3. ValueFunction: (position, stepNumber, diagonalOrOrthogonalStep, previousValue) => int
                //  4. FloodUntil: (allPositionsWithValues) => int (stepNumber)
                //  5. ReturnPositionsFilter: (position, value, allPositionsWithValues) => bool
                //)

                // For each monster
                //      If monster already at max attack range from hero and in line of sight of hero, continue
                //      Find all possible squares that can be walked to - WalkDistanceFrom (monsters can walk through but not end on monsters)
                //          :PlotValuesByFloodSearch(MonsterPosition, Walls, () => previousValue + D*3 + O*2), Max(Values) >= MonsterMovement, !monsters.Contains(position) && value <= monsterMovement)
                //          If no walkable squares, continue          
                //      Check if any in range and in line of sight of hero. If yes, find best (max range, then lowest movements required). Move there, end.

                //      Otherwise, find empty squares in attack range from hero with line of sight -> (priority: order by attack range desc)
                //          :PlotValuesByFloodSearch(HeroPosition, levelBorder, () => previousValue + D*3 + O*2) + (has wall ? int.Max : 0) /* filters out walls */ + (!(has line of sight) ? int.Max : 0), Max(Values) >= MonsterAttackRange, !(monsters excluding self).Contains(position) && value <= monsterAttackRange)
                //      Otherwise, find empty squares closest to hero ignoring monsters -> (priority: order by distance to hero asc)
                //          :PlotValuesByFloodSearch(HeroPosition, walls, () => previousValue + D*3 + O*2), positions.Any(p => !(walls+monsters).Contains(p)), value > min(value where position is empty))
                //      For each optimal square, find closest walkable square OR current position
                //          : PlotValuesByFloodSearch(OptimalSquare, walls, () => previousValue + D*3 + O*2), positions.Any(p => target.Contains(p)), target.Contains(position))
                //      Choose walkable square based on walk distance from optimal square. Break tie by movements required, otherwise just get first

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.MonstersAttack)
            {
                // Find all monsters in line of sight and in range, add up monster attack, divide by hero defence points, decrease hero health

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.MonsterActionsEnd)
            {
                // Simply move to LevelEnd
            }

            throw new NotImplementedException();
        }
    }
}
