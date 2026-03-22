using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

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

        var currentState = _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(expected));

        Assert.That(currentState.GamePhase, Is.EqualTo(GamePhase.HeroActions));
    }

    [Test]
    public void WhenGetGameSnapshot_ThenReturnsExpected()
    {
        var expected = new GameState()
        {
            Hero = new Hero()
            {
                Name = "Alfred"
            }
        };

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(expected));

        var snapshot = _sut.GetGameStateSnapshot();

        var snapshotDeserialized = JsonSerializer.Deserialize<GameState>(snapshot, SerializationUtility.JsonSerializerOptions);

        Assert.That(snapshotDeserialized!.Hero.Name, Is.EqualTo("Alfred"));
    }
}
