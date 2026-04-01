using System;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models.Geometry
{
    public struct Point
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = GeometryUtility.Snap(x);
            Y = GeometryUtility.Snap(y);
        }

        public Point Translate(double xDelta, double yDelta)
        {
            return new Point(X + xDelta, Y + yDelta);
        }

        public override bool Equals(Object other)
        {
            return other is Point && this == (Point) other;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        public static bool operator ==(Point point, Point other)
        {
            return point.X == other.X && point.Y == other.Y;
        }

        public static bool operator !=(Point point, Point other)
        {
            return !(point == other);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public double DistanceFrom(Point point)
        {
            return (double) Math.Sqrt(Math.Pow((double)(point.X - X), 2) + Math.Pow((double)(point.Y - Y), 2));
        }

        public bool IsCornerPoint()
        {
            return (Math.Abs(X - Math.Round(X)) < GameConstants.GeometryCalculationEpsilon) 
                && (Math.Abs(Y - Math.Round(Y)) < GameConstants.GeometryCalculationEpsilon);
        }
    }
}
