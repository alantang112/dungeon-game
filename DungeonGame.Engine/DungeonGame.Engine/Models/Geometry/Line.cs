namespace DungeonGame.Engine.Models.Geometry
{
    public struct Line
    {
        public Point PointA { get; }
        public Point PointB { get; }

        public Line(Point pointA, Point pointB)
        {
            PointA = pointA;
            PointB = pointB;
            Gradient = (pointB.Y - pointA.Y)/(pointB.X - pointA.X);
            YIntercept = pointA.Y - Gradient * pointA.X;
        }

        public decimal Gradient { get; }
        public decimal YIntercept { get; }

        public decimal GetYAtX(decimal x) => Gradient * x + YIntercept;
        public decimal GetXAtY(decimal y) => (y - YIntercept) / Gradient;
    }
}
