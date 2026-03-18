using System;

namespace DungeonGame.Engine.Models.Geometry
{
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position Translate(int xDelta, int yDelta)
        {
            return new Position(X + xDelta, Y + yDelta);
        }

        public override bool Equals(Object other)
        {
            return other is Position && this == (Position) other;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        public static bool operator ==(Position position, Position other)
        {
            return position.X == other.X && position.Y == other.Y;
        }

        public static bool operator !=(Position position, Position other)
        {
            return !(position == other);
        }
    }
}
