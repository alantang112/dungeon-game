using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class MonsterActionsTests
{
    private IGameEngine _sut;

    private int _heroInitialHealth = 10;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();

        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.MonsterActions,
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

        initialGameState.World.InitializeLevel(1);

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
    public void WhenContinue_ThenGoToNextGamePhase()
    {
        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.MonsterActionsEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.EnergyDicePreRoll));
    }
}
