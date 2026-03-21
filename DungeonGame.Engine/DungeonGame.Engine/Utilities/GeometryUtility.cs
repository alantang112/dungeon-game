using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Utilities
{
    public class GeometryUtility
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

        #region LineOfSight
        public static bool HasLineOfSightOf(Position observerPosition, Position targetPosition, Position[] blockers)
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

        private static bool HasLineOfSightOfInAStraightLine(Position observerPosition, Position targetPosition, Position[] blockers)
        {
            if (!IsHorizontalOrVerticalLine(observerPosition, targetPosition) && !IsDiagonalLine(observerPosition, targetPosition))
                throw new ArgumentException("Input positions are not in a straight or diagonal line");

            var position = observerPosition;

            // check all squares in between observer and target
            while (position != targetPosition)
            {
                var checkPositionXDelta = targetPosition.X == position.X ? 0 : ((targetPosition.X > position.X) ? 1 : -1);
                var checkPositionYDelta = targetPosition.Y == position.Y ? 0 : ((targetPosition.Y > position.Y) ? 1 : -1);

                var checkPosition = position.Translate(checkPositionXDelta, checkPositionYDelta);

                if (checkPosition != targetPosition && blockers.Any(blocker => blocker == checkPosition))
                    return false;

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
                // Get positions bounded by the intercepts along the line, excluding observer and target
                var positionsBoundedByInterceptsAlongLine = GetPositionsBoundedByInterceptsAlongLine(orderedInterceptsAlongLine)
                        .Where(p => p != observerPosition && p != targetPosition)
                        .ToList();
                if (positionsBoundedByInterceptsAlongLine.Any(p => blockers.Contains(p)))
                {
                    continue; // this line is blocked so continue to check the next vertex pair
                }                

                // Check 2: If line crosses at an corner, check if there is a perpendicular wall blocking line of sight  
                // Find all points where (x,y) are (int, int), filter out points outside the bounded positions
                var orderedInterceptsAlongLineAtCorners = FilterOutPointsOutsideBoundedPositions(orderedInterceptsAlongLine, positionsBoundedByInterceptsAlongLine)
                        .Where(p => p.IsCornerPoint()).ToList();
                // Find the perpendicular positions above and below the line, filter out pairs where perp square has observer or target
                var perpendicularPositions = orderedInterceptsAlongLineAtCorners
                        .Select(p => GetPerpendicularPositionsToLineAtCornerPoint(p, line))
                        .Where(pp => pp.Above != observerPosition && pp.Below != targetPosition)
                        .ToList(); 

                // If any(perp. square above) is blocked AND any(perp. square below) is blocked, this line is blocked
                if (perpendicularPositions.Any(pp => blockers.Contains(pp.Above)) && perpendicularPositions.Any(pp => blockers.Contains(pp.Below)))
                {
                    continue; // this line is blocked so continue to check the next vertex pair
                }
                
                // If Check 1 and Check 2 passes, then there is line of sight
                return true;
            }

            return false;
        }

        private static bool IsHorizontalOrVerticalLine(Point pointA, Point pointB)
        {
            return pointA.X == pointB.X || pointA.Y == pointB.Y;
        }

        private static bool IsHorizontalOrVerticalLine(Position positionA, Position positionB)
        {
            return positionA.X == positionB.X || positionA.Y == positionB.Y;
        }

        private static bool IsDiagonalLine(Position positionA, Position positionB)
        {
            return Math.Abs(positionA.X - positionB.X) == Math.Abs(positionA.Y - positionB.Y);
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

        private static List<Point> FilterOutPointsOutsideBoundedPositions(List<Point> points, List<Position> boundedPositions)
        {
            if (!boundedPositions.Any())
                return points;

            var minX = boundedPositions[0].X;
            var maxX = boundedPositions[0].X + 1; // need to +1 because the position is a box
            var minY = boundedPositions[0].Y;
            var maxY = boundedPositions[0].Y + 1; // need to +1 because the position is a box

            for (int i = 1; i < boundedPositions.Count(); i++)
            {
                var position = boundedPositions[i];

                if (position.X < minX) minX = position.X;
                else if (position.X + 1 > maxX) maxX = position.X + 1;

                if (position.Y < minY) minY = position.Y;
                else if (position.Y + 1 > maxY) maxY = position.Y + 1;
            }

            return points.Where(p => p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY).ToList();
        }

        private static PerpendicularPositions GetPerpendicularPositionsToLineAtCornerPoint(Point point, Line line)
        {
            if (!point.IsCornerPoint())
                throw new ArgumentException($"Point is not a corner point: ({point.X}, {point.Y})");
            if (line.Gradient == 0)
                throw new ArgumentException("GetPerpendicularPositionsToLineAtCornerPoint should not be called for gradient=0 lines");

            // if gradient is positive, get top left and bottom right
            if (line.Gradient > 0)
            {
                return new PerpendicularPositions(new Position((int)point.X - 1, (int)point.Y), new Position((int)point.X, (int)point.Y - 1));
            }
            else // if gradient is negative, get bottom left and top right
            {
                return new PerpendicularPositions(new Position((int)point.X, (int)point.Y), new Position((int)point.X - 1, (int)point.Y - 1));
            }
        }
        #endregion
    }
}
