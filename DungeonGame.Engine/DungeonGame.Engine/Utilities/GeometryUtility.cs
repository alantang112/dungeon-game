using System;
using System.Collections.Generic;
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
        /// <param name="valueFunction">Function to evaluate "value" of position. Inputs: (currentPosition, stepNumber, isDiagonalStep, previousPosition, previousValue)</param>
        /// <param name="floodUntilStepNumber">Return final step number if possible to determine. Inputs: (allPositionsWithValue)</param>
        /// <param name="returnPositionsFilter">Return true if currentPosition should be returned in results: Inputs: (currentPosition, value, allPositionsWithValues)</param>
        /// <returns>Result contains the position and the calculated value</returns>
        public static Dictionary<Position, int> PlotValuesByFloodSearch(
            Position seed, List<Position> blockers, Func<Position, int, bool, Position, (int, int), (int, int)> valueFunction, Func<Dictionary<Position, int>, int?> floodUntilStepNumber, 
            Func<Position, int, Dictionary<Position, (int,int)>, bool> returnPositionsFilter)
        {
            var floodSearchResults = new Dictionary<Position, (int, int)> { { seed, (0, 0) } };
            var floodSearchResultsPreviousCount = floodSearchResults.Count;
            
            int? finalStepNumber = null;
            var stepNumber = 0;

            while(true)
            {
                var positionsToFloodFrom = floodSearchResults.Where(x => x.Value.Item1 == stepNumber).ToList();

                if (!positionsToFloodFrom.Any() && stepNumber > floodSearchResults.Where(x => x.Value.Item1 != int.MaxValue).Max(x => x.Value.Item1))
                {
                    break;
                }

                foreach((Position position, (int, int) value) in positionsToFloodFrom)
                {
                    // walk diagonally then orthogonally
                    foreach((int xDelta, int yDelta) in DiagonalStepDirections.Concat(OrthogonalStepDirections))
                    {
                        var newPosition = position.Translate(xDelta, yDelta);
                        var isDiagonal = Math.Abs(xDelta) + Math.Abs(yDelta) == 2;

                        // check it is not diagonally blocked
                        if (isDiagonal && blockers.Contains(new Position(position.X, newPosition.Y)) && blockers.Contains(new Position(newPosition.X, position.Y)))
                            continue;

                        var hasExistingResult = floodSearchResults.TryGetValue(newPosition, out var existingFloorSearchResult);

                        if (hasExistingResult && existingFloorSearchResult.Item2 == 0)
                        {
                            continue;
                        }

                        if (!blockers.Contains(newPosition))
                        {
                            var newPositionValue = valueFunction(newPosition, stepNumber, isDiagonal, position, value);

                            if (hasExistingResult)
                            {
                                floodSearchResults[newPosition] = 
                                (
                                    newPositionValue.Item1 < existingFloorSearchResult.Item1 ? newPositionValue.Item1 : existingFloorSearchResult.Item1,
                                    newPositionValue.Item2 < existingFloorSearchResult.Item2 ? newPositionValue.Item2 : existingFloorSearchResult.Item2
                                ); 
                            }
                            else
                            {
                                floodSearchResults.Add(newPosition, newPositionValue);
                            }

                        }
                    }
                }

                // evaluate positions
                if (finalStepNumber.HasValue && stepNumber >= finalStepNumber.Value)
                    break;

                stepNumber++;

                if (stepNumber >= GameConstants.LoopIterationLimit)
                    throw new InvalidOperationException("Potential infinite loop in PlotValuesByFloodSearch");
            }

            var returnFloodSearchResults = new Dictionary<Position, int>();

            foreach((Position position, (int,int) value) in floodSearchResults)
            {
                if (returnPositionsFilter(position, value.Item1, floodSearchResults))
                    returnFloodSearchResults.Add(position, value.Item1);
            }

            return returnFloodSearchResults;
        }

        public static List<Position> FindWalkPath(Dictionary<Position, int> walkablePositions, Position start, Position end)
        {
            if (!walkablePositions.ContainsKey(end))
                throw new ArgumentException("Walkable positions should contain the end position");
        
            var path = new List<Position>()
            {
                end
            };

            while (true)
            {
                var newStepFound = false;

                // try walk diagonally first
                foreach(var direction in DiagonalStepDirections)
                {
                    if (FindWalkPathTryWalk(direction, start, walkablePositions, path))
                    {
                        newStepFound = true;
                        break;
                    }
                }

                // try walk orthogonally
                if (!newStepFound)
                {
                    foreach(var direction in OrthogonalStepDirections)
                    {
                        if (FindWalkPathTryWalk(direction, start, walkablePositions, path))
                        {
                            newStepFound = true;
                            break;
                        }
                    }
                }
                
                if (newStepFound)
                {
                    if (path.Last() == start)
                    {
                        path.Reverse();
                        return path;
                    }

                    continue;
                }

                throw new InvalidOperationException("Could not find next step in FindWalkPath");
            }
        }

        private static bool FindWalkPathTryWalk((int, int) direction, Position start, Dictionary<Position, int> walkablePositions, List<Position> path) 
        {
            var currentPosition = path.Last();
            var currentValue = walkablePositions[currentPosition];

            var newPosition = currentPosition.Translate(direction.Item1, direction.Item2);

            if (newPosition != start && !walkablePositions.ContainsKey(newPosition))
                return false;

            var movementRequired = currentValue - (newPosition == start ? 0 : walkablePositions[newPosition]);

            if (movementRequired == ((Math.Abs(direction.Item1) + Math.Abs(direction.Item2) == 2)  ? GameConstants.MovementPointsDiagonal : GameConstants.MovementPointsOrthogonal))
            {
                path.Add(newPosition);

                return true;
            }

            return false;
        }

        public static bool HasWallIsland(List<Position> walls)
        {
            return walls.Any(wall => IsIsland(walls, wall));
        }

        private static bool IsIsland(List<Position> walls, Position start)
        {
            var queue = new Queue<Position>();
            queue.Enqueue(start);

            var visited = new HashSet<Position>
            {
                start
            };

            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                foreach(var stepDirection in OrthogonalStepDirections.Concat(DiagonalStepDirections))
                {
                    var newPosition = position.Translate(stepDirection.Item1, stepDirection.Item2);

                    if (newPosition.X == 0 || newPosition.X == (GameConstants.LevelSize + 1) || newPosition.Y == 0 || newPosition.Y == (GameConstants.LevelSize + 1))
                        return false;

                    if (walls.Contains(newPosition) && !visited.Contains(newPosition))
                    {
                        queue.Enqueue(newPosition);
                        visited.Add(newPosition);
                    }
                }

            }

            return true;
        }

        public static List<Position> GetNeighbouringPositions(Position position)
        {
            var result = new List<Position>();

            foreach(var stepDirection in OrthogonalStepDirections.Concat(DiagonalStepDirections))
            {
                result.Add(position.Translate(stepDirection.Item1, stepDirection.Item2));
            }

            return result;
        }

        private static (int, int)[] DiagonalStepDirections => new (int, int)[] { (1, 1), (1, -1), (-1, -1), (-1, 1) };
        private static (int, int)[] OrthogonalStepDirections => new (int, int)[] { (1, 0), (0, -1), (-1, 0), (0, 1) }; 
    }
}
