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

                // Check 1: If any square crossed by line is blocked, this line is blocked
                var orderedInterceptsAlongLine = GetOrderedInterceptsAlongLine(line, vertexPair.ObserverVertex, vertexPair.TargetVertex);
                var positionsBoundedByInterceptsAlongLine = GetPositionsBoundedByInterceptsAlongLine(orderedInterceptsAlongLine);
                if (positionsBoundedByInterceptsAlongLine.Where(p => p != observerPosition && p != targetPosition).Any(p => blockers.Contains(p)))
                {
                    continue; // this line is blocked so continue to check the next vertex pair
                }                

                /// Check 2:
                /// Find all points where (x,y) are (int, int)
                var orderedInterceptsAlongLineAtCorners = orderedInterceptsAlongLine; // TODO
                /// Find the perpendicular squares above and below
                /// Filter out pairs where perp square has observer or target
                /// Filter out points that is only connected to a line segment that is occupied by an observer or target
                /// If any(perp. square above) is blocked AND any(perp. square below) is blocked, this line is blocked
                /// 
                
                // If Check 1 is unblocked AND Check 2 is unblocked, then there is line of sight       
                return true;
            }

            return false;
        }

        private static bool IsHorizontalOrVerticalLine(Point pointA, Point pointB)
        {
            return pointA.X == pointB.X || pointA.Y == pointB.Y;
        }

        private static List<Point> GetOrderedInterceptsAlongLine(Line line, Point start, Point end)
        {
            var minX = Math.Min(start.X, end.X);
            var maxX = Math.Max(start.X, end.X);
            var minY = Math.Min(start.Y, end.Y);
            var maxY = Math.Max(start.Y, end.Y);

            var pointsAlongLine = new HashSet<Point>();

            for (var x = minX; x <= maxX; x++)
            {
                pointsAlongLine.Add(new Point(x, line.GetYAtX(x)));
            }
            for (var y = minY; y <= maxY; y++)
            {
                pointsAlongLine.Add(new Point(line.GetXAtY(y), y));
            }

            var orderedPointsAlongLine = pointsAlongLine.OrderBy(x => x.DistanceFrom(start)).ToList();

            return orderedPointsAlongLine;
        }

        private static List<Position> GetPositionsBoundedByInterceptsAlongLine(List<Point> intercepts)
        {
            var positions = new HashSet<Position>();

            for (var i = 0; i < intercepts.Count(); i++)
            {
                if (i == (intercepts.Count() - 1))
                    continue; // we can actually skip the last point

                var point1 = intercepts[i];
                var point2 = intercepts[i + 1];

                var position = new Position(
                    (int) Math.Floor(Math.Min(point1.X, point2.X)),
                    (int) Math.Floor(Math.Min(point1.Y, point2.Y))
                );

                positions.Add(position);
            }

            return positions.ToList();
        }

        public static double Snap(double value)
        {
            double snapped = Math.Round(value / GameConstants.GeometryCalculationEpsilon) * GameConstants.GeometryCalculationEpsilon;
            return Math.Round(snapped, GameConstants.GeometryCalculationDecimalPlaces);
        }
    }
}
