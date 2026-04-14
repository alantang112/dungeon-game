using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class NextLevelTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();

        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.LevelEnd,
            EnergyDice = new EnergyDice(),
            LevelNumber = 1,
            World = new World(),
            Hero = new Hero()
            {
                Health = 8,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, 2 }
                },
            }
        };

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);
        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }

    [Test]
    public void GivenNextLevel_WhenNextLevel_ThenContinue()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.NextLevel
        });

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.UpgradeHero));
        Assert.That(gameState.LevelNumber, Is.EqualTo(2));
        Assert.That(gameState.World.UpgradePointsAvailable, Is.EqualTo(1));
        Assert.That(gameState.World.Monsters.Count, Is.GreaterThan(0));
    }
}
