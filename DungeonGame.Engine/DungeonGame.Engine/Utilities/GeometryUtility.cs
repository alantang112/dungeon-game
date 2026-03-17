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

        public static bool HasLineOfSightOf(Position observer, Position target, Position[] blockers)
        {
            if (observer.Equals(target))
            {
                throw new ArgumentException($"observer and target are in the same position");
            }

            // straight line case
            if (observer.X == target.X)
            {
                var minYToCheck = Math.Min(observer.Y, target.Y) + 1;
                var maxYToCheck = Math.Max(observer.Y, target.Y) - 1;

                for (var y = minYToCheck; y <= maxYToCheck; y++)
                {
                    if (blockers.Any(blocker => blocker.X == observer.X && blocker.Y == y))
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (observer.Y == target.Y)
            {
                var minXToCheck = Math.Min(observer.X, target.X) + 1;
                var maxXToCheck = Math.Max(observer.X, target.X) - 1;

                for (var x = minXToCheck; x <= maxXToCheck; x++)
                {
                    if (blockers.Any(blocker => blocker.X == x && blocker.Y == observer.Y))
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
