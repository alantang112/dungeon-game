using System;
using System.Linq;
using DungeonGame.Engine.Models;

namespace DungeonGame.Engine.Utilities
{
    public class GeometryUtility
    {
        public static int CalculateDistanceBetween(Position position1, Position position2)
        {
            var xAbsDelta = Math.Abs(position1.X - position2.X);
            var yAbsDelta = Math.Abs(position1.Y - position2.Y);

            // straight up/down/left/right:
            if (xAbsDelta == 0 || yAbsDelta == 0)
            {
                return (xAbsDelta + yAbsDelta) * 2;
            }
            // diagonal:
            else if (xAbsDelta == yAbsDelta)
            {
                return xAbsDelta * 3;
            }
            // everything else:
            else if (xAbsDelta > yAbsDelta)
            {
                return (xAbsDelta * 2) + yAbsDelta;
            }
            else if (yAbsDelta > xAbsDelta)
            {
                return (yAbsDelta * 2) + xAbsDelta;
            }

            throw new NotSupportedException("Code should never reach here");
        }

        public static bool HasLineOfSightOf(Position observerPosition, Position targetPosition, Position[] blockers)
        {
            if (observerPosition == targetPosition)
            {
                throw new ArgumentException($"observer and target are in the same position");
            }

            // straight line case (horizontal, vertical, diagonal)
            if (observerPosition.X == targetPosition.X 
                || observerPosition.Y == targetPosition.Y 
                || Math.Abs(observerPosition.X - targetPosition.X) == Math.Abs(observerPosition.Y - targetPosition.Y))
            {
                return HasLineOfSightOfInAStraightLine(observerPosition, targetPosition, blockers);
            }

            // all else
            throw new NotImplementedException();
        }

        private static bool HasLineOfSightOfInAStraightLine(Position observerPosition, Position targetPosition, Position[] blockers)
        {
            if (observerPosition.X != targetPosition.X 
                && observerPosition.Y != targetPosition.Y 
                && Math.Abs(observerPosition.X - targetPosition.X) != Math.Abs(observerPosition.Y - targetPosition.Y))
                throw new ArgumentException("Input positions are not in a straight line or diagonal");

            var diagonalLine = Math.Abs(observerPosition.X - targetPosition.X) == Math.Abs(observerPosition.Y - targetPosition.Y);

            var position = observerPosition;

            // check all squares in between observer and target
            while (position != targetPosition)
            {
                var checkPositionXDelta = targetPosition.X == position.X ? 0 : ((targetPosition.X > position.X) ? 1 : -1);
                var checkPositionYDelta = targetPosition.Y == position.Y ? 0 : ((targetPosition.Y > position.Y) ? 1 : -1);

                var checkPosition = position.Translate(checkPositionXDelta, checkPositionYDelta);

                if (blockers.Any(blocker => blocker == checkPosition))
                    return false;

                // handle diagonal case
                if (diagonalLine)
                {
                    var checkPosition1 = position.Translate(checkPositionXDelta, 0);
                    var checkPosition2 = position.Translate(0, checkPositionYDelta);

                    if (blockers.Any(blocker => blocker == checkPosition1) && blockers.Any(blocker => blocker == checkPosition2))
                        return false;
                }

                // update position
                position = checkPosition;

                // TODO check the final diagonal
            }

            return true;
        } 
    }
}
