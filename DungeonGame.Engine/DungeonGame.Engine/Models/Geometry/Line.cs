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

        public double Gradient { get; }
        public double YIntercept { get; }

        public double GetYAtX(double x) => Gradient * x + YIntercept;
        public double GetXAtY(double y) => (y - YIntercept) / Gradient;
    }
}
