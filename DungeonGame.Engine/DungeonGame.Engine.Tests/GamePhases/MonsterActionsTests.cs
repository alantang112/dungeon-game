using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Tests.GamePhases;

public class MonsterActionsTests
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
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, 2 }
                },
            }
        };

        initialGameState.World.InitializeLevel(1);

        initialGameState.World.Monsters.RemoveAt(1);

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState);
        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }   

    #region MonsterMove
    [Test]
    public void MonsterMove_GivenMonsterAlreadyAtMaxAttackRange_AndInLineOfSight_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 4);
        initialGameState.World.Monsters[0].Position = new Position(4, 3);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
    }

    [Test]
    public void MonsterMove_GivenMonsterHasNoWalkableSquares_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(4, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
    }

    [Test]
    public void MonsterMove_GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_ButCurrentSquareIsBetter_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Monsters[0].Monster.Stats[SkillType.AttackRange] = 4;
        initialGameState.World.Walls.Add(new Position(3, 4));
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
    }

    [Test]
    public void MonsterMove_GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_AndIsBetterThanCurrentSquare_ThenMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(2, 1);
        initialGameState.World.Monsters[0].Position = new Position(5, 3);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(3, 2)));
    }

    [Test]
    public void MonsterMove_GivenNoWalkableSquaresInAttackRangeAndLineOfSight_ThenMoveToClosestSquareAtMaxAttackRangeAndLineSight()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(2, 1);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
    }

    [Test]
    public void MonsterMove_GivenNoWalkableSquaresInAttackRangeAndLineOfSight_AndNoEmptySquaresInAttackRangeAndLineOfSight_ThenMoveAsCloseAsPossibleToHero()
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
                    { SkillType.AttackRange, 2 }
                }
            },
            Position = new Position(1, 2)
        });

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState));

        _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(1, 3)));
    }
    #endregion
}
