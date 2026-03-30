using DungeonGame.Engine.Models;

namespace DungeonGame.Engine.Tests.Models;

public class WorldTests
{
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    public void CanInitializeLevel(int levelNumber)
    {
        var world = new World();

        world.InitializeLevel(levelNumber);
    }
}