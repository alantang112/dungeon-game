using System;

namespace DungeonGame.Engine.Models.Geometry
{
    public struct Point
    {
        public decimal X { get; }
        public decimal Y { get; }

        public Point(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public Point Translate(decimal xDelta, decimal yDelta)
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

        public decimal DistanceFrom(Point point)
        {
            return (decimal) Math.Sqrt(Math.Pow((double)(point.X - X), 2) + Math.Pow((double)(point.Y - Y), 2));
        }
    }
}
