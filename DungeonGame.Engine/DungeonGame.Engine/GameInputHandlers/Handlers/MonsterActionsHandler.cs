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
                // TODO: WalkDistanceFrom Utility method (start, blockers, maxSteps) -> Dictionary<Position, int>
                // TODO: InRangeFrom Utility method (start, blockers, range) -> Dictionary<Position, int>
                
                // For each monster
                //      If monster already at max attack range from hero, continue
                //      Find all possible squares that can be walked to - WalkDistanceFrom (monsters can walk through but not end on monsters)
                //      Find candidate destinations:
                //          Find empty squares in attack range from hero with line of sight -> store listA (priority: order by attack range desc) - InRangeFrom + HasLineOfSightOf(blockers: walls + monsters)
                //          If none, find empty squares closest to hero ignoring monsters -> store listC (priority: order by distance to hero asc) - WalkDistanceFrom(hero, walls, levelSize ** 2)
                //      For each walkable square, calculate the min distance to an optimal destination (use flood algorithm). It should take into account walls (all lists) and monsters (listA only)
                //      Choose the square that has the min distance. If tied, order by walkDistance from starting position (i.e. walk the minimum steps)

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
