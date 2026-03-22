using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.Tests.GamePhases;

public class StartPhaseTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
    }

    [Test]
    public void WhenStartGame_SetHeroName()
    {
        var gameState = _sut.ProcessInput(new InputEvent() { EventType = InputEventType.NewGame, EventParameters = new NewGameEventParameters() { HeroName = "Conan" } });

        Assert.That(gameState.Hero.Name, Is.EqualTo("Conan"));
    }

    [Test]
    public void WhenStartGame_InitializeLevelOne()
    {
        var gameState = _sut.ProcessInput(new InputEvent() { EventType = InputEventType.NewGame, EventParameters = new NewGameEventParameters() { HeroName = "Conan" } });

        Assert.That(gameState.LevelNumber, Is.EqualTo(1));
        Assert.That(gameState.World.HeroPosition, Is.EqualTo(new Position(1, 1)));
        Assert.That(gameState.World.Walls.Count(), Is.EqualTo(27));
        Assert.That(gameState.World.Monsters.Count(), Is.EqualTo(2));

        // Hero stats initialized
        Assert.That(gameState.Hero.Stats[SkillType.Movement], Is.EqualTo(1));
        Assert.That(gameState.Hero.Stats[SkillType.Attack], Is.EqualTo(1));
        Assert.That(gameState.Hero.Stats[SkillType.Defence], Is.EqualTo(1));
        Assert.That(gameState.Hero.Stats[SkillType.AttackRange], Is.EqualTo(2));

        Assert.That(gameState.Hero.Health, Is.EqualTo(10));

        Assert.That(gameState.World.HeroActionPoints, Is.Empty);
    }

    [Test]
    public void GivenGameInStartPhase_WhenNewGameInput_ThenGoToEnergyDicePreRoll()
    {
        var currentState = _sut.ProcessInput(new InputEvent() { EventType = InputEventType.NewGame, EventParameters = new NewGameEventParameters() { HeroName = "Conan" } });
        Assert.That(currentState.GamePhase, Is.EqualTo(GamePhase.EnergyDicePreRoll));
    }

    [Test]
    public void GivenGameInStartPhase_WhenNotNewGameInput_ThenThrowNotSupportedException()
    {
        var invalidInputEventTypes = Enum.GetValues<InputEventType>()
                                         .Where(x => x != InputEventType.NewGame)
                                         .ToList();

        foreach(var invalidInputEventType in invalidInputEventTypes)
        {
            Assert.Throws<NotSupportedException>(() => _sut.ProcessInput(new InputEvent() { EventType = invalidInputEventType }));
        }
    }
}
