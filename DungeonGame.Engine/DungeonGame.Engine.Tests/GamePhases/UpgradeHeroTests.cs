using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class UpgradeHeroTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();

        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.UpgradeHero,
            EnergyDice = new EnergyDice(),
            LevelNumber = 1,
            World = new World()
            {
                UpgradePointsAvailable = 1
            },
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
    public void GivenUpgradeHero_AndNoParameterProvided_ThenThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.UpgradeHero,
            EventParameters = null
        }));
    }

    [Test]
    public void GivenUpgradeHero_AndNoSkillTypeOrReplenishHealth_ThenReturnGameMessage()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.UpgradeHero,
            EventParameters = new UpgradeHeroEventParameters()
            {
                SkillType = null,
                ReplenishHealth = false
            }
        });

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.UpgradeHero));
        Assert.That(gameState.GameMessage, Is.EqualTo(GameMessages.LevelUpError));
    }

    [Test]
    public void GivenUpgradeHero_AndSkillTypeAndReplenishHealth_ThenReturnGameMessage()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.UpgradeHero,
            EventParameters = new UpgradeHeroEventParameters()
            {
                SkillType = SkillType.Movement,
                ReplenishHealth = true
            }
        });

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.UpgradeHero));
        Assert.That(gameState.GameMessage, Is.EqualTo(GameMessages.LevelUpError));
    }

    [TestCase(SkillType.Movement, false)]
    [TestCase(SkillType.Attack, false)]
    [TestCase(SkillType.Defence, false)]
    [TestCase(SkillType.AttackRange, false)]
    [TestCase(null, true)]
    public void GivenUpgradeHero_AndSkillTypeOrReplenishHealth_ThenProceed(SkillType? skillType, bool replenishHealth)
    {
        var initialGameState = _sut.GetCurrentState(); 

        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.UpgradeHero,
            EventParameters = new UpgradeHeroEventParameters()
            {
                SkillType = skillType,
                ReplenishHealth = replenishHealth
            }
        });

        if (skillType.HasValue)
        {
            Assert.That(gameState.Hero.Stats[skillType.Value], Is.EqualTo(initialGameState.Hero.Stats[skillType.Value] + 1));
            Assert.That(gameState.Hero.Health, Is.EqualTo(8));
        }
        else if (replenishHealth)
        {
            Assert.That(gameState.Hero.Health, Is.EqualTo(GameConstants.HeroMaxHealth));
        }
        else
        {
            throw new NotSupportedException("Invalid test case");
        }
            
        foreach(SkillType otherSkill in Enum.GetValues<SkillType>().Where(x => x != skillType))
        {
            Assert.That(gameState.Hero.Stats[otherSkill], Is.EqualTo(initialGameState.Hero.Stats[otherSkill]));
        }

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.EnergyDicePreRoll));
    }
}