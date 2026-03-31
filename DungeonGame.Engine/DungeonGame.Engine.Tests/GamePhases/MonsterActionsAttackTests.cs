using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class MonsterActionsAttackTests
{
    private IGameEngine _sut;

    private int _heroInitialHealth = 10;

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
                Health = _heroInitialHealth,
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

        initialGameState.World.Monsters.ForEach(m => {
            m.Monster.Stats[SkillType.Movement] = 0;
            m.Monster.Stats[SkillType.AttackRange] = 4;
        });

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
    public void GivenNoMonstersInRange_ThenDoNotLoseHealth()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.Monsters[0].Position = new Position(1, 5);
        initialGameState.World.Monsters[1].Position = new Position(5, 1);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.Hero.Health, Is.EqualTo(_heroInitialHealth));
    }

    [Test]
    public void GivenNoMonstersInLineOfSight_ThenDoNotLoseHealth()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.Monsters[0].Position = new Position(3, 1);
        initialGameState.World.Monsters[1].Position = new Position(3, 2);

        initialGameState.World.Walls.Add(new Position(2, 1));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.Hero.Health, Is.EqualTo(_heroInitialHealth));
    }

    [TestCase(5, 0)]
    [TestCase(4, 1)]
    [TestCase(3, 1)]
    [TestCase(2, 2)]
    [TestCase(1, 4)]
    public void GivenOneMonstersInRangeAndLineOfSight_ThenMonsterAttack(int heroDefence, int expectedHealthLoss)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.Monsters[0].Position = new Position(1, 2);
        initialGameState.World.Monsters[1].Position = new Position(1, 3);

        initialGameState.World.HeroActionPoints[SkillType.Defence] = heroDefence;

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.Hero.Health, Is.EqualTo(_heroInitialHealth - expectedHealthLoss));
    }

    [TestCase(9, 0)]
    [TestCase(8, 1)]
    [TestCase(7, 1)]
    [TestCase(6, 1)]
    [TestCase(5, 1)]
    [TestCase(4, 2)]
    [TestCase(3, 2)]
    [TestCase(2, 4)]
    [TestCase(1, 8)]
    public void GivenTwoMonstersInRangeAndLineOfSight_ThenMonsterAttack(int heroDefence, int expectedHealthLoss)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.Monsters[0].Position = new Position(1, 3);
        initialGameState.World.Monsters[1].Position = new Position(3, 1);

        initialGameState.World.HeroActionPoints[SkillType.Defence] = heroDefence;

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.Hero.Health, Is.EqualTo(_heroInitialHealth - expectedHealthLoss));
    }

    [TestCase(4, false)]
    [TestCase(3, true)]
    [TestCase(2, true)]
    [TestCase(1, true)]
    public void GivenMonsterDealEnoughDamageToBringHeroHealthZeroOrBelow_GoToGameEnd(int heroDefence, bool gameEndExpected)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.Monsters[0].Position = new Position(1, 3);
        initialGameState.World.Monsters[1].Position = new Position(3, 1);

        initialGameState.World.Monsters[0].Monster.Stats[SkillType.Attack] = 15;
        initialGameState.World.Monsters[1].Monster.Stats[SkillType.Attack] = 15;

        initialGameState.World.HeroActionPoints[SkillType.Defence] = heroDefence;

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(gameEndExpected ? GamePhase.GameEnd : GamePhase.MonsterActions));
    }
}
