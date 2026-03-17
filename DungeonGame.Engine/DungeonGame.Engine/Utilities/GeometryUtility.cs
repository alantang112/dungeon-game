using System;
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

            // straight line case
            if (observerPosition.X == targetPosition.X)
            {
                var minYToCheck = Math.Min(observerPosition.Y, targetPosition.Y) + 1;
                var maxYToCheck = Math.Max(observerPosition.Y, targetPosition.Y) - 1;

                for (var y = minYToCheck; y <= maxYToCheck; y++)
                {
                    if (blockers.Any(blocker => blocker.X == observerPosition.X && blocker.Y == y))
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (observerPosition.Y == targetPosition.Y)
            {
                var minXToCheck = Math.Min(observerPosition.X, targetPosition.X) + 1;
                var maxXToCheck = Math.Max(observerPosition.X, targetPosition.X) - 1;

                for (var x = minXToCheck; x <= maxXToCheck; x++)
                {
                    if (blockers.Any(blocker => blocker.X == x && blocker.Y == observerPosition.Y))
                    {
                        return false;
                    }
                }

                return true;
            }

            throw new NotImplementedException();
        }
    }
}
