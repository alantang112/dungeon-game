using System.Text.Json;
using DungeonGame.Engine.Models;

namespace DungeonGame.Engine.Tests;

public class GameEngineTests
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
        var currentState = _sut.GetCurrentState();
        Assert.That(currentState.GamePhase, Is.EqualTo(GamePhase.Start));
    }

    [Test]
    public void WhenLoadGameState_ThenGameStateSet()
    {
        var expected = new GameState()
        {
            GamePhase = GamePhase.HeroActions
        };

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(expected));

        var currentState = _sut.GetCurrentState();

        Assert.That(currentState.GamePhase, Is.EqualTo(GamePhase.HeroActions));
    }

    [Test]
    public void WhenGetGameSnapshot_ThenReturnsExpected()
    {
        var expected = new GameState()
        {
            GamePhase = GamePhase.MonstersAttack
        };

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(expected));

        var snapshot = _sut.GetGameStateSnapshot();

        var snapshotDeserialized = JsonSerializer.Deserialize<GameState>(snapshot);

        Assert.That(snapshotDeserialized!.GamePhase, Is.EqualTo(GamePhase.MonstersAttack));
    }
}
