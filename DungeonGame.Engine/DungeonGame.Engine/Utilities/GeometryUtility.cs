using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Utilities
{
    public partial class GeometryUtility
    {
        public static double Snap(double value)
        {
            double snapped = Math.Round(value / GameConstants.GeometryCalculationEpsilon) * GameConstants.GeometryCalculationEpsilon;
            return Math.Round(snapped, GameConstants.GeometryCalculationDecimalPlaces);
        }

        public static int CalculateDistanceBetween(Position position1, Position position2)
        {
            var xAbsDelta = Math.Abs(position1.X - position2.X);
            var yAbsDelta = Math.Abs(position1.Y - position2.Y);

            // straight up/down/left/right:
            if (xAbsDelta == 0 || yAbsDelta == 0)
            {
                return (xAbsDelta + yAbsDelta) * GameConstants.MovementPointsOrthogonal;
            }
            // diagonal:
            else if (xAbsDelta == yAbsDelta)
            {
                return xAbsDelta * GameConstants.MovementPointsDiagonal;
            }
            // everything else:
            else if (xAbsDelta > yAbsDelta)
            {
                return (xAbsDelta * GameConstants.MovementPointsOrthogonal) + yAbsDelta * (GameConstants.MovementPointsDiagonal - GameConstants.MovementPointsOrthogonal);
            }
            else if (yAbsDelta > xAbsDelta)
            {
                return (yAbsDelta * GameConstants.MovementPointsOrthogonal) + xAbsDelta * (GameConstants.MovementPointsDiagonal - GameConstants.MovementPointsOrthogonal);
            }

            throw new NotSupportedException("Code should never reach here");
        }

        public static bool HasLineOfSightOf(Position observerPosition, Position targetPosition, List<Position> blockers)
        {
            if (observerPosition == targetPosition)
            {
                throw new ArgumentException($"observer and target are in the same position");
            }

            // straight line case (horizontal, vertical)
            if (IsHorizontalOrVerticalLine(observerPosition, targetPosition))
            {
                return HasLineOfSightOfInAStraightLine(observerPosition, targetPosition, blockers);
            }

            // diagonal case: always needs an unblocked direct path similar to straight line case
            // in addition to the 'complex' scenario rules
            if (IsDiagonalLine(observerPosition, targetPosition) && !HasLineOfSightOfInAStraightLine(observerPosition, targetPosition, blockers))
                return false;

            // all else
            return HasLineOfSightOfComplexLine(observerPosition, targetPosition, blockers);
        }

        /// <summary>
        /// Find positions by flood search.
        /// Algorithm: For step X, loop through all positions with value=X, step diagonally then step orthogonally
        /// ignoring blockers, then evaluate using valueFunction. Then assess floodUntilStepNumber, if result returned,
        /// then terminate after performing that step number. Can also terminate if between two steps the number of 
        /// positions has not changed.
        /// Once final step is done, filter out positions using returnPositionsFilter and return.
        /// </summary>
        /// <param name="seed">Starting position</param>
        /// <param name="blockers">Positions to ignore for flood search</param>
        /// <param name="valueFunction">Function to evaluate "value" of position. Inputs: (currentPosition, stepNumber, isDiagonalStep, previousValue)</param>
        /// <param name="floodUntilStepNumber">Return final step number if possible to determine. Inputs: (allPositionsWithValue)</param>
        /// <param name="returnPositionsFilter">Return true if currentPosition should be returned in results: Inputs: (currentPosition, value, allPositionsWithValues)</param>
        /// <returns>Result contains the position and the calculated value</returns>
        public static Dictionary<Position, int> PlotValuesByFloodSearch(
            Position seed, List<Position> blockers, Func<Position, int, bool, int, int> valueFunction, Func<Dictionary<Position, int>, int?> floodUntilStepNumber, 
            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter)
        {
            var floodSearchResults = new Dictionary<Position, int> { { seed, 0 } };
            var floodSearchResultsPreviousCount = floodSearchResults.Count;
            
            int? finalStepNumber = null;
            var stepNumber = 0;

            while(true)
            {
                var positionsToFloodFrom = floodSearchResults.Where(x => x.Value == stepNumber).ToList();

                if (!positionsToFloodFrom.Any() && stepNumber > floodSearchResults.Where(x => x.Value != int.MaxValue).Max(x => x.Value))
                {
                    break;
                }

                foreach((Position position, int value) in positionsToFloodFrom)
                {
                    // walk diagonally
                    foreach((int xDelta, int yDelta) in DiagonalStepDirections)
                    {
                        var newPosition = position.Translate(xDelta, yDelta);

                        // check it is not blocked
                        if (blockers.Contains(new Position(position.X, newPosition.Y)) && blockers.Contains(new Position(newPosition.X, position.Y)))
                            continue;

                        if (floodSearchResults.ContainsKey(newPosition))
                            continue;

                        if (!blockers.Contains(newPosition))
                        {
                            var newPositionValue = valueFunction(newPosition, stepNumber, true, value);
                            floodSearchResults.Add(newPosition, newPositionValue);
                        }
                    }

                    // walk orthogonally
                    foreach((int xDelta, int yDelta) in OrthogonalStepDirections)
                    {
                        var newPosition = position.Translate(xDelta, yDelta);

                        if (floodSearchResults.ContainsKey(newPosition))
                            continue;

                        if (!blockers.Contains(newPosition))
                        {
                            var newPositionValue = valueFunction(newPosition, stepNumber, false, value);
                            floodSearchResults.Add(newPosition, newPositionValue);
                        }
                    }
                }

                // evaluate positions
                if (finalStepNumber.HasValue && stepNumber >= finalStepNumber.Value)
                    break;

                stepNumber++;

                if (stepNumber >= 100)
                    throw new InvalidOperationException("Potential infinite loop in PlotValuesByFloodSearch");
            }

            var returnFloodSearchResults = new Dictionary<Position, int>();

            foreach((Position position, int value) in floodSearchResults)
            {
                if (returnPositionsFilter(position, value, floodSearchResults))
                    returnFloodSearchResults.Add(position, value);
            }

            return returnFloodSearchResults;
        }

        #region Flood Search Helpers 
        
        #endregion

        private static (int, int)[] DiagonalStepDirections => new (int, int)[] { (1, 1), (1, -1), (-1, -1), (-1, 1) };
        private static (int, int)[] OrthogonalStepDirections => new (int, int)[] { (1, 0), (0, -1), (-1, 0), (0, 1) }; 
    }
}
