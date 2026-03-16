namespace DungeonGame.Engine.Tests.Utilities;

public class GeometryUtilityTests
{
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
        // TODO randomise the starting position
        throw new NotImplementedException();
    }
    #endregion

    #region HasLineOfSightOf
    /*
    * Scenarios:
    * Straight lines + blocked
    * Diagonal lines + blocked
    * Complex lines
    */
    #endregion
}
