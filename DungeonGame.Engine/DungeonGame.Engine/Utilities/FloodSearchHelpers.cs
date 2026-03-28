using System;
using System.Collections.Generic;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Utilities
{
    public class FloodSearchHelpers
    {
        public static int WalkValueFunction(Position position, int stepNumber, bool isDiagonalStep, int previousValue)
            => previousValue + (isDiagonalStep ? GameConstants.MovementPointsDiagonal : GameConstants.MovementPointsOrthogonal);

        public static Func<Dictionary<Position, int>, int?> FloodUntilAllSquaresWalked = (Dictionary<Position, int> floodValues) 
            => null;
        public static Func<Position, int, Dictionary<Position, int>, bool> ReturnAllPositions = (Position position, int value, Dictionary<Position, int> floodValues) 
            => true;
    }
}
