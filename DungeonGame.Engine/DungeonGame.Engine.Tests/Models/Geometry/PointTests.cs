using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Tests.Models.Geometry;

public class PointTests
{
    [TestCase(0, 0, 0, 0)]
    [TestCase(0.000001, -0.000001, 0, 0)]
    [TestCase(0.00001, -0.00001, 0.00001, -0.00001)]
    [TestCase(1, 1, 1, 1)]
    [TestCase(1.000001, 0.999999, 1, 1)]
    [TestCase(1.00001, 0.99999, 1.00001, 0.99999)]
    [TestCase(-1, -1, -1, -1)]
    [TestCase(-1.000001, -0.999999, -1, -1)]
    [TestCase(-1.00001, -0.99999, -1.00001, -0.99999)]
    public void WhenCreatePoint_XYAreSnapped(double x, double y, double expectedX, double expectedY)
    {
        var point = new Point(x, y);
        Assert.That(point.X, Is.EqualTo(expectedX));
        Assert.That(point.Y, Is.EqualTo(expectedY));
    }

    [TestCase(0, 0, true)]
    [TestCase(0.000001, 0, true)]
    [TestCase(0.00001, 0, false)]
    [TestCase(-0.00001, 0, false)]
    [TestCase(0,0.00001, false)]
    [TestCase(0,-0.00001, false)]
    [TestCase(1, 1, true)]
    [TestCase(1.000001, 1, true)]
    [TestCase(0.999999, 1, true)]
    [TestCase(1.00001, 0, false)]
    [TestCase(-1.00001, 0, false)]
    [TestCase(0,1.00001, false)]
    [TestCase(0,-1.00001, false)]
    public void IsCornerPoint_ReturnsCorrectly(double x, double y, bool expected)
    {
        var point = new Point(x, y);
        Assert.That(point.IsCornerPoint(), Is.EqualTo(expected));
    }
}
