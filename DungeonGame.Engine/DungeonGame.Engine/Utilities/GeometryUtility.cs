using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models.Geometry;

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
            return HasLineOfSightOfComplexLine(observerPosition, targetPosition, blockers);
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

                if (checkPosition != targetPosition && blockers.Any(blocker => blocker == checkPosition))
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
            }

            return true;
        } 

        private static bool HasLineOfSightOfComplexLine(Position observerPosition, Position targetPosition, Position[] blockers)
        {
            var observerVertices = new Point[] { 
                new Point(observerPosition.X, observerPosition.Y),  
                new Point(observerPosition.X, observerPosition.Y + 1), 
                new Point(observerPosition.X + 1, observerPosition.Y), 
                new Point(observerPosition.X + 1, observerPosition.Y + 1)
            };

            var targetVertices = new Point[] { 
                new Point(targetPosition.X, targetPosition.Y),  
                new Point(targetPosition.X, targetPosition.Y + 1), 
                new Point(targetPosition.X + 1, targetPosition.Y), 
                new Point(targetPosition.X + 1, targetPosition.Y + 1)
            };

            var vertexPairs = observerVertices.SelectMany(observerVertex => targetVertices, (o, t) => new { ObserverVertex = o, TargetVertex = t })
                                                .Where(p => !IsHorizontalOrVerticalLine(p.ObserverVertex, p.TargetVertex))
                                                .ToArray();

            foreach(var vertexPair in vertexPairs)
            {
                var line = new Line(vertexPair.ObserverVertex, vertexPair.TargetVertex);

                /// Check 1: 
                /// Find all points that cross x or y
                /// Order by distance from observer
                /// Walk along line and find all squares it goes through
                /// Filter out if contains observer or target
                /// Id any of these are blocked, this line is blocked
                var pointsAlongLine = new HashSet<Point>();
                for (var x = vertexPair.ObserverVertex.X; x <= vertexPair.TargetVertex.X; x++)
                {
                    pointsAlongLine.Add(new Point(x, line.GetYAtX(x)));
                }
                for (var y = vertexPair.ObserverVertex.Y; y <= vertexPair.TargetVertex.Y; y++)
                {
                    pointsAlongLine.Add(new Point(line.GetXAtY(y), y));
                }

                var orderedPointsAlongLine = pointsAlongLine.OrderBy(x => x.DistanceFrom(vertexPair.ObserverVertex)).ToList();

                /// 
                /// Check 2:
                /// Find all points where (x,y) are (int, int)
                /// Find the perpendicular squares above and below
                /// Filter out pairs where perp square has observer or target
                /// Filter out points that is only connected to a line segment that is occupied by an observer or target
                /// If any(perp. square above) is blocked AND any(perp. square below) is blocked, this line is blocked
                /// 
                /// If Check 1 is unblocked AND Check 2 is unblocked, then there is line of sight
            }

            return false;
        }

        private static bool IsHorizontalOrVerticalLine(Point pointA, Point pointB)
        {
            return (pointA.X == pointB.X || pointA.Y == pointB.Y);
        }
    }
}
