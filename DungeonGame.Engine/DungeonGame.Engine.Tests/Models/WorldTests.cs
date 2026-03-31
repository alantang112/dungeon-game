using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

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

    private static HashSet<Position> ParsePositionsString(string s)
    {
        var result = new HashSet<Position>();

        var positions = s.Split('|');

        foreach(var position in positions.Where(x => !string.IsNullOrEmpty(x)))
        {
            var split = position.Split(',');
            result.Add(new Position(int.Parse(split[0]), int.Parse(split[1])));
        }

        return result;
    }

    [TestCase(1, 1, 0, 5, 5, 5, 4, 1, 5, "")]
    [TestCase(1, 1, 2, 5, 5, 5, 4, 1, 5, "1,2|2,1")]
    [TestCase(2, 4, 2, 5, 5, 5, 4, 1, 5, "2,5|3,4|2,3|1,4")]
    [TestCase(2, 4, 3, 5, 5, 5, 4, 1, 5, "2,5|3,5|3,4|3,3|2,3|1,3|1,4")]
    [TestCase(5, 4, 3, 5, 5, 5, 3, 1, 5, "4,3|4,5")]
    public void Test_CalculateHeroCanWalkPositions(int heroX, int heroY, int movementPoints, int monsterX, int monsterY, int monster2X, int monster2Y, int wallX, int wallY, string expectedCanWalkPositions)
    {
        var sut = new GameEngine();
        
        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.HeroActions,
            EnergyDice = new EnergyDice(),
            LevelNumber = 1,
            World = new World(),
            Hero = new Hero()
            {
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, 2 }
                },
            }
        };

        initialGameState.World.InitializeLevel(1, initRandomWalls: false);
        initialGameState.World.HeroPosition = new Engine.Models.Geometry.Position(heroX, heroY);

        initialGameState.World.HeroActionPoints = new Dictionary<SkillType, int>()
            {
                { SkillType.Movement, movementPoints },
                { SkillType.Attack, 1 },
                { SkillType.Defence, 1 }
            };

        initialGameState.World.Monsters[0].Position = new Engine.Models.Geometry.Position(monsterX, monsterY);
        initialGameState.World.Monsters[1].Position = new Engine.Models.Geometry.Position(monster2X, monster2Y);

        initialGameState.World.Walls.Add(new Engine.Models.Geometry.Position(wallX, wallY));

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);

        sut.LoadGameStateSnapshot(initialGameStateJson);

        var gameState = sut.GetCurrentState();

        var expected = ParsePositionsString(expectedCanWalkPositions);
        var actual = gameState.ViewData.HeroCanWalkPositions.ToHashSet();

        Assert.That(actual.Count, Is.EqualTo(expected.Count));
        Assert.That(expected.All(e => actual.Contains(e)), Is.True);
    }

    [TestCase(1,1, 4, 2, 5,5, 4,5, 1,5, "")]
    [TestCase(5,4, 3, 2, 5,5, 4,5, 1,5, "")]
    [TestCase(5,4, 4, 2, 5,5, 4,5, 1,5, "5,5")]
    [TestCase(5,4, 4, 3, 5,5, 4,5, 1,5, "5,5")]
    [TestCase(5,3, 4, 4, 5,5, 5,4, 1,5, "5,4")]
    [TestCase(4,3, 4, 3, 5,4, 5,2, 1,5, "5,4|5,2")]
    public void Test_CalculateHeroCanAttackPositions(int heroX, int heroY, int attackPoints, int attackRange, int monsterX, int monsterY, int monster2X, int monster2Y, int wallX, int wallY, string expectedCanAttackPositions)
    {
        var sut = new GameEngine();
        
        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.HeroActions,
            EnergyDice = new EnergyDice(),
            LevelNumber = 1,
            World = new World(),
            Hero = new Hero()
            {
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, attackRange }
                },
            }
        };

        initialGameState.World.InitializeLevel(1, initRandomWalls: false);
        initialGameState.World.HeroPosition = new Engine.Models.Geometry.Position(heroX, heroY);

        initialGameState.World.HeroActionPoints = new Dictionary<SkillType, int>()
            {
                { SkillType.Movement, 1 },
                { SkillType.Attack, attackPoints },
                { SkillType.Defence, 1 }
            };

        initialGameState.World.Monsters[0].Position = new Engine.Models.Geometry.Position(monsterX, monsterY);
        initialGameState.World.Monsters[1].Position = new Engine.Models.Geometry.Position(monster2X, monster2Y);

        initialGameState.World.Walls.Add(new Engine.Models.Geometry.Position(wallX, wallY));

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);

        sut.LoadGameStateSnapshot(initialGameStateJson);

        var gameState = sut.GetCurrentState();

        var expected = ParsePositionsString(expectedCanAttackPositions);
        var actual = gameState.ViewData.HeroCanAttackPositions.ToHashSet();

        Assert.That(actual.Count, Is.EqualTo(expected.Count));
        Assert.That(expected.All(e => actual.Contains(e)), Is.True);
    }
}