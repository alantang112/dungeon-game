using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Utilities
{
    public class FloodSearchHelpers
    {
        public static (int,int) WalkValueFunction(Position position, int stepNumber, bool isDiagonalStep, Position previousPosition, (int,int) previousValue)
            => (previousValue.Item1 + (isDiagonalStep ? GameConstants.MovementPointsDiagonal : GameConstants.MovementPointsOrthogonal), 0);

        public static Func<Position, int, bool, Position, (int,int), (int,int)> WalkValueFunctionConsideringWalkingThroughMonsters(int movementStat, Position[] monsters)
        {
            Func<Position, int, bool, Position, (int,int), (int,int)> walkFunction = (Position position, int stepNumber, bool isDiagonalStep, Position previousPosition, (int,int) previousValue) => {
                var result = WalkValueFunction(position, stepNumber, isDiagonalStep, previousPosition, previousValue);

                // check if we are moving into or moving out of a position occupied by a monster
                if (monsters.Contains(position) || monsters.Contains(previousPosition))
                {
                    result.Item2 = previousValue.Item2 + (isDiagonalStep ? GameConstants.MovementPointsDiagonal : GameConstants.MovementPointsOrthogonal);

                    if (result.Item2 > movementStat)
                    {
                        result.Item1 = int.MaxValue;
                    }
                }
                
                return result;
            };

            return walkFunction;
        }

        public static Func<Dictionary<Position, int>, int?> FloodUntilAllSquaresWalked = (Dictionary<Position, int> floodValues) 
            => null;
        public static Func<Position, int, Dictionary<Position, (int,int)>, bool> ReturnAllValidPositions = (Position position, int value, Dictionary<Position, (int,int)> floodValues) 
            => value < int.MaxValue;
    }
}
