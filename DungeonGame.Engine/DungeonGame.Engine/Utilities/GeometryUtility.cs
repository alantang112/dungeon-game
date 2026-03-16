using System;
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
            throw new NotImplementedException();
        }
    }
}
