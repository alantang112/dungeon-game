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

        public double DistanceFrom(Point point)
        {
            return (double) Math.Sqrt(Math.Pow((double)(point.X - X), 2) + Math.Pow((double)(point.Y - Y), 2));
        }
    }
}
