using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class MonsterActionsMoveTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();

        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.HeroActions,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { null, null, null }
            },
            LevelNumber = 1,
            World = new World(),
            Hero = new Hero()
            {
                Health = 10,
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

        initialGameState.World.Monsters.RemoveAt(1);

        initialGameState.World.HeroActionPoints = new Dictionary<SkillType, int>()
        {
            { SkillType.Movement, 0 },
            { SkillType.Attack, 0 },
            { SkillType.Defence, 7 },
        };

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);
        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }   

    [Test]
    public void GivenMonsterAlreadyAtMaxAttackRange_AndInLineOfSight_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 4);
        initialGameState.World.Monsters[0].Position = new Position(4, 3);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
    }

    [Test]
    public void GivenMonsterHasNoWalkableSquares_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(4, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(5, 5)));
    }

    [Test]
    public void GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_ButCurrentSquareIsBetter_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Monsters[0].Monster.Stats[SkillType.AttackRange] = 4;
        initialGameState.World.Walls.Add(new Position(3, 4));
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(5, 5)));
    }

    [TestCase(2, 1, 5, 3, null, null, 3, 2)]
    [TestCase(3, 4, 2, 4, null, null, 2, 3)] // (2,5) is also acceptable
    [TestCase(1, 1, 3, 3, 3, 1, 1, 2)]
    public void GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_AndIsBetterThanCurrentSquare_ThenMove(int heroX, int heroY, int monsterX, int monsterY, int? extraWallX, int? extraWallY, int expectedMonsterX, int expectedMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        if (extraWallX.HasValue)
        {
            initialGameState.World.Walls.Add(new Position(extraWallX.Value, extraWallY!.Value));
        }

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
    }

    [TestCase(2, 1, 5, 5, 4, 3)]
    [TestCase(4, 1, 5, 4, 5, 2)]
    [TestCase(1, 3, 4, 5, 2, 4)]
    public void GivenNoWalkableSquaresInAttackRangeAndLineOfSight_ThenMoveToClosestSquareAtMaxAttackRangeAndLineSight(int heroX, int heroY, int monsterX, int monsterY, int expectedMonsterX, int expectedMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
    }

    [TestCase(2, 1, 5, 5, 4, 5, null, null, 4, 3, 3, 3)]
    [TestCase(1, 1, 3, 2, 5, 3, 1, 3, 2, 1, 3, 2)] // or 4, 1
    [TestCase(1, 1, 2, 3, 1, 3, 2, 1, 1, 2, 1, 3)]
    public void GivenTwoMonsters_NoWalkableSquaresInAttackRangeAndLineOfSight_ThenMoveToClosestSquareAtMaxAttackRangeAndLineSight(int heroX, int heroY, int monsterX, int monsterY, 
        int otherMonsterX, int otherMonsterY, int? extraWallX, int? extraWallY, int expectedMonsterX, int expectedMonsterY, int expectedOtherMonsterX, int expectedOtherMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = initialGameState.World.Monsters[0].Monster,
            Position = new Position(otherMonsterX, otherMonsterY)
        });

        if (extraWallX.HasValue)
        {
            initialGameState.World.Walls.Add(new Position(extraWallX.Value, extraWallY!.Value));
        }

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
        Assert.That(newGameState.World.Monsters[1].Position, Is.EqualTo(new Position(expectedOtherMonsterX, expectedOtherMonsterY)));
    }

    [Test]
    public void GivenNoWalkableSquaresInAttackRangeAndLineOfSight_AndNoEmptySquaresInAttackRangeAndLineOfSight_ThenMoveAsCloseAsPossibleToHero()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(1, 1);
        initialGameState.World.Monsters[0].Position = new Position(2, 5);
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = new Monster()
            {
                Health = 1,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 0 },
                    { SkillType.Attack, 2 },
                    { SkillType.AttackRange, 2 }
                }
            },
            Position = new Position(2, 1)
        });
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = new Monster()
            {
                Health = 1,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 0 },
                    { SkillType.Attack, 2 },
                    { SkillType.AttackRange, 2 }
                }
            },
            Position = new Position(1, 2)
        });

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(1, 3)));
    }
}
