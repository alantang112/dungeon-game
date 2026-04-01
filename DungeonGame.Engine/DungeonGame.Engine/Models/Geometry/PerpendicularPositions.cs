namespace DungeonGame.Engine.Models.Geometry
{
    public struct PerpendicularPositions
    {
        public Position Above { get; }
        public Position Below { get; }

        public PerpendicularPositions(Position above, Position below)
        {
            Above = above;
            Below = below;
        }

        public override string ToString()
        {
            return $"({Above}, {Below})";
        }
    }
}
