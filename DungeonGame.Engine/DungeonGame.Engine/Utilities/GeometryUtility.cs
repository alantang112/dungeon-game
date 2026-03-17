using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models;

namespace DungeonGame.Engine.Utilities
{
    public class GeometryUtility
    {
        public static int CalculateDistanceBetween(Position position1, Position position2)
        {
            var xDelta = Math.Abs(position1.X - position2.X);
            var yDelta = Math.Abs(position1.Y - position2.Y);

            // straight up/down/left/right:
            if (xDelta == 0 || yDelta == 0)
            {
                return (xDelta + yDelta) * 2;
            }
            // diagonal:
            else if (xDelta == yDelta)
            {
                return xDelta * 3;
            }
            // everything else:
            else if (xDelta > yDelta)
            {
                return (xDelta * 2) + yDelta;
            }
            else if (yDelta > xDelta)
            {
                return (yDelta * 2) + xDelta;
            }

            throw new NotSupportedException("Code should never reach here");
        }

        public static bool HasLineOfSightOf(Position observerPosition, Position targetPosition, Position[] blockers)
        {
            if (observerPosition == targetPosition)
            {
                throw new ArgumentException($"observer and target are in the same position");
            }

            // straight line case (horizontal, vertical)
            if (observerPosition.X == targetPosition.X || observerPosition.Y == targetPosition.Y)
            {
                var checkPosition = observerPosition;
                // check all squares in between observer and target
                while (checkPosition != targetPosition)
                {
                    if (checkPosition.X != targetPosition.X)
                        checkPosition.X += (targetPosition.X > checkPosition.X) ? 1 : -1;

                    if (checkPosition.Y != targetPosition.Y)
                        checkPosition.Y += (targetPosition.Y > checkPosition.Y) ? 1 : -1;

                    if (blockers.Any(blocker => blocker == checkPosition))
                        return false;
                }

                return true;
            }

            // diagonal line case
            if (Math.Abs(observerPosition.X - targetPosition.X) == Math.Abs(observerPosition.Y - targetPosition.Y))
            {
                
            }

            // all else

            throw new NotImplementedException();
        }
    }
}
