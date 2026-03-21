using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.Utilities;

public class GeometryUtilityTests
{
    private Position RandomPosition()
    {
        return new Position(RandomUtility.RandomInt(1, 10), RandomUtility.RandomInt(1, 10));
    }

    #region CalculateDistanceBetween
    
    // same position
    [TestCase(0, 0, 0)]
    // orthogonal neighbour
    [TestCase(0, 1, 2)]
    [TestCase(0, -1, 2)]
    [TestCase(1, 0, 2)]
    [TestCase(-1, 0, 2)]
    // diagonal neighbour
    [TestCase(1, 1, 3)]
    [TestCase(1, -1, 3)]
    [TestCase(-1, 1, 3)]
    [TestCase(-1, -1, 3)]
    // orthogonal 
    [TestCase(0, 2, 4)]
    [TestCase(0, -3, 6)]
    [TestCase(4, 0, 8)]
    [TestCase(-5, 0, 10)]
    // diagonal
    [TestCase(2, 2, 6)]
    [TestCase(3, -3, 9)]
    [TestCase(-4, 4, 12)]
    [TestCase(-5, -5, 15)]
    // complex
    [TestCase(1, 2, 5)]
    [TestCase(3, 1, 7)]
    [TestCase(4, -1, 9)]
    [TestCase(2, -6, 14)]
    [TestCase(-3, -8, 19)]
    [TestCase(-10, -4, 24)]
    [TestCase(-7, 3, 17)]
    [TestCase(-2, 5, 12)]
    public void CalculateDistanceBetween_Calculate(int xDelta, int yDelta, int expected)
    {
        var position = RandomPosition();
        var OtherPosition = position.Translate(xDelta, yDelta);

        var actual = GeometryUtility.CalculateDistanceBetween(position, OtherPosition);
        
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion

    #region HasLineOfSightOf
    
    private void TestLineOfSight(int targetXDelta, int targetYDelta, string wallDeltas, bool expected)
    {
        var observer = new Position(0, 0); // TODO: RandomPosition();

        var mirrorX = 1; // TODO RandomUtility.RandomBool() ? 1 : -1;
        var mirrorY = 1; // TODO RandomUtility.RandomBool() ? 1 : -1;

        var target = observer.Translate(targetXDelta * mirrorX, targetYDelta * mirrorY);

        var blockers = new List<Position>();
        foreach(var wallDelta in wallDeltas.Split("|").Where(x => !string.IsNullOrEmpty(x)))
        {
            var wallDeltaSplit = wallDelta.Split(",");
            var wall = observer.Translate(int.Parse(wallDeltaSplit[0]) * mirrorX, int.Parse(wallDeltaSplit[1]) * mirrorY);
            blockers.Add(wall);
        }

        var actual = GeometryUtility.HasLineOfSightOf(observer, target, blockers);

        Assert.That(actual, Is.EqualTo(expected));
    }

    // immediate neighbour
    [TestCase(0, 1, "0,2", true)]
    [TestCase(1, 0, "1,1", true)]
    [TestCase(0, -1, "0,1", true)]
    [TestCase(-1, 0, "-3,2", true)]
    // 1 gap
    [TestCase(0, 2, "1,2", true)]
    [TestCase(2, 0, "1,0", false)]
    // 2 gap
    [TestCase(0, -3, "1,0", true)]
    [TestCase(-3, 0, "-2,0", false)]
    public void HasLineOfSightOf_GivenTargetInStraightLines_ThenCalculateInLineOfSight(int targetXDelta, int targetYDelta, string wallDeltas, bool expected)
    {
        TestLineOfSight(targetXDelta, targetYDelta, wallDeltas, expected);
    }

    // immediate neighbour
    [TestCase(1, 1, "", true)]
    [TestCase(1, 1, "0,1", true)]
    [TestCase(1, 1, "1,0", true)]
    [TestCase(1, 1, "0,1|1,0", false)]
    // 1 gap
    [TestCase(2, 2, "", true)]
    [TestCase(2, 2, "1,1", false)]
    [TestCase(2, 2, "2,1|1,2", false)]
    [TestCase(2, 2, "0,1|1,0", false)]
    [TestCase(2, 2, "1,0|1,2", true)]
    [TestCase(2, 2, "0,1|2,1", true)]
    // 2 gap
    [TestCase(3, 3, "", true)]
    [TestCase(3, 3, "1,1", false)]
    [TestCase(3, 3, "2,2", false)]
    [TestCase(3, 3, "3,2|2,3", false)]
    [TestCase(3, 3, "3,2", true)]
    [TestCase(3, 3, "2,3", true)]
    [TestCase(3, 3, "2,1|1,2", false)]
    [TestCase(3, 3, "2,1", true)]
    [TestCase(3, 3, "1,2", true)]
    [TestCase(3, 3, "0,1|1,0", false)]
    [TestCase(3, 3, "0,1", true)]
    [TestCase(3, 3, "1,0", true)]
    [TestCase(3, 3, "4,3|3,4", true)]
    [TestCase(3, 3, "1,0|1,2", true)]
    [TestCase(3, 3, "0,1|2,1", true)]
    [TestCase(3, 3, "2,1|2,3", true)]
    [TestCase(3, 3, "2,3|0,1", true)]
    [TestCase(3, 3, "1,2|3,2", true)]
    [TestCase(3, 3, "2,1|2,3|0,1", false)]
    public void HasLineOfSightOf_GivenTargetInDiagonalLines_ThenCalculateInLineOfSight(int targetXDelta, int targetYDelta, string wallDeltas, bool expected)
    {
        TestLineOfSight(targetXDelta, targetYDelta, wallDeltas, expected);
    }

    // (2,1) scenarios
    [TestCase(2, 1, "", true)]
    [TestCase(2, 1, "1,0|1,1", false)]
    [TestCase(2, 1, "2,0|1,1", false)]
    [TestCase(2, 1, "1,0|0,1", false)]
    [TestCase(2, 1, "0,2|1,2|2,2|2,0|0,1", true)]
    [TestCase(2, 1, "0,2|1,2|2,2|2,0|1,0", true)]
    [TestCase(2, 1, "0,1|0,-1|1,-1|2,-1|1,1", true)]
    [TestCase(2, 1, "0,1|0,-1|1,-1|2,-1|2,0", true)]
    // (3,1) scenarios
    [TestCase(3, 1, "", true)]
    [TestCase(3, 1, "1,0|0,1", false)]
    [TestCase(3, 1, "1,0|1,1", false)]
    [TestCase(3, 1, "2,0|1,1", false)]
    [TestCase(3, 1, "2,0|2,1", false)]
    [TestCase(3, 1, "3,0|2,1", false)]
    [TestCase(3, 1, "-1,0|-1,-1|0,-1|1,-1|2,-1|3,-1|4,0|4,1|4,2|3,2|2,2|1,1|0,1|-1,1|2,1", true)]
    [TestCase(3, 1, "-1,0|-1,-1|0,-1|1,-1|2,-1|3,-1|4,0|4,1|4,2|3,2|2,2|1,1|0,1|-1,1|3,0", true)]
    [TestCase(3, 1, "-1,0|-1,-1|0,-1|1,-1|2,0|3,0|4,0|4,1|4,2|3,2|2,2|1,2|0,2|-1,1|0,1", true)]
    [TestCase(3, 1, "-1,0|-1,-1|0,-1|1,-1|2,0|3,0|4,0|4,1|4,2|3,2|2,2|1,2|0,2|-1,1|1,0", true)]
    // (3,2) scenarios
    [TestCase(3, 2, "", true)]
    [TestCase(3, 2, "1,0|0,1", false)]
    [TestCase(3, 2, "1,0|1,1", false)]
    [TestCase(3, 2, "2,1|1,1", false)]
    [TestCase(3, 2, "2,0|1,1", false)]
    [TestCase(3, 2, "3,1|2,2", false)]
    [TestCase(3, 2, "1,1|3,1", false)]
    [TestCase(3, 2, "2,1|0,1", false)]
    [TestCase(3, 2, "2,1|1,1", false)]
    [TestCase(3, 2, "2,1|1,2", false)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|1,0|2,0|2,1|3,1|4,1|4,2|4,3|3,3|2,3|1,3|0,3|0,2|-1,2|-1,1|-1,0", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|1,0|2,0|3,0|4,1|4,2|4,3|3,3|2,3|2,2|1,2|0,2|-1,2|-1,1|-1,0", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|1,0|2,0|3,0|3,1|4,1|4,2|4,3|3,3|2,3|1,2|0,2|-1,2|-1,1|-1,0", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,0|3,0|4,1|4,2|4,3|3,3|2,3|1,2|0,2|-1,1|-1,0|0,1|2,2", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,0|3,0|4,1|4,2|4,3|3,3|2,3|1,2|0,2|-1,1|-1,0|1,0|3,1", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,0|3,1|4,1|4,2|4,3|3,3|2,3|1,3|0,2|-1,1|-1,0|0,1|1,2", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,0|3,1|4,1|4,2|4,3|3,3|2,3|1,3|0,2|-1,1|-1,0|1,0|2,1", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,-1|3,0|4,1|4,2|4,3|3,3|2,3|2,2|1,2|1,1|0,1|-1,1|-1,0", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,-1|3,0|4,0|4,1|4,2|4,3|3,3|2,3|2,2|1,2|0,1|-1,1|-1,0|1,1", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,-1|3,0|4,0|4,1|4,2|4,3|3,3|2,3|2,2|1,2|0,1|-1,1|-1,0|2,0", true)]
    [TestCase(3, 2, "-1,-1|-1,0|1,-1|2,-1|3,0|4,1|4,2|4,3|3,3|2,3|1,2|0,1|-1,1|-1,0|1,1|2,2", true)]
    [TestCase(3, 2, "-1,-1|-1,0|1,-1|2,-1|3,0|4,1|4,2|4,3|3,3|2,3|1,2|0,1|-1,1|-1,0|2,0|3,1", true)]
    [TestCase(3, 2, "-1,-1|0,-1|1,-1|2,-1|2,0|3,0|3,1|4,1|4,2|4,3|3,3|2,3|1,3|1,2|0,2|0,1|-1,1|-1,0", true)]
    public void HasLineOfSightOf_GivenTargetInComplexLines_ThenCalculateInLineOfSight(int targetXDelta, int targetYDelta, string wallDeltas, bool expected)
    {
        TestLineOfSight(targetXDelta, targetYDelta, wallDeltas, expected);
    }
    #endregion

    #region Snap
    [TestCase(0.0, 0.0)]
    [TestCase(-0.0000001, 0.0)]
    [TestCase(0.0000001, 0.0)]
    [TestCase(1.0, 1.0)]
    [TestCase(1.0000001, 1.0)]
    [TestCase(0.9999999, 1.0)]
    [TestCase(2.0, 2.0)]
    [TestCase(2.0000001, 2.0)]
    [TestCase(1.9999999, 2.0)]
    [TestCase(1.5, 1.5)]
    [TestCase(-1.0, -1.0)]
    [TestCase(-1.0000001, -1.0)]
    [TestCase(-0.9999999, -1.0)]
    [TestCase(-2.0, -2.0)]
    [TestCase(-2.0000001, -2.0)]
    [TestCase(-1.9999999, -2.0)]
    [TestCase(-1.5, -1.5)]
    public void Snap_GivenValue_ThenSnap(double value, double expected)
    {
        Assert.That(GeometryUtility.Snap(value), Is.EqualTo(expected));
    }
    #endregion
}
