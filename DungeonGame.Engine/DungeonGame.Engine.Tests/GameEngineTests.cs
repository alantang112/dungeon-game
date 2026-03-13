using DungeonGame.Engine;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.Tests;

public class Tests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
    }

    [Test]
    public void GivenNewGameEngine_ThenInitWithStartPhase()
    {
        var gameState = _sut.CurrentState;
        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.Start));
    }

    [Test]
    public void GivenGameInStartPhase_WhenNewGame_ThenReceiveGameChangedEvent()
    {
        var gameChangedCount = 0;
        var gameChangedListener = () => { gameChangedCount++; };

        _sut.OnStateChanged += gameChangedListener;
        _sut.ProcessInput(new InputEvent() { EventType = InputEventType.NewGame, EventParameters = new NewGameEventParameters() { HeroName = "Conan" } });

        Assert.That(gameChangedCount, Is.EqualTo(1));
    }

    [Test]
    public void GivenGameInStartPhase_WhenNewGameInput_ThenGoToEnergyDicePreRoll()
    {
        _sut.ProcessInput(new InputEvent() { EventType = InputEventType.NewGame, EventParameters = new NewGameEventParameters() { HeroName = "Conan" } });
        Assert.That(_sut.CurrentState.GamePhase, Is.EqualTo(GamePhase.EnergyDicePreRoll));
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
