using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Tests.GamePhases;

public class EnergyDicePreRollTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
        
        var initialGameState = JsonSerializer.Serialize(new GameState()
        {
            GamePhase = GamePhase.EnergyDicePreRoll,
        });

        _sut.LoadGameStateSnapshot(initialGameState);
    }

    [Test]
    public void WhenRoll_ThenInitializeEnergyDice()
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        var gameState = _sut.GetCurrentState();

        Assert.That(gameState.EnergyDice.Dice.All(x => x >= 1 && x <= 6), Is.True);
    }

    [Test]
    public void WhenRoll_ThenUpdateGamePhaseToEnergyDiceAssignment()
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        var gameState = _sut.GetCurrentState();

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.EnergyDiceAssignment));
    }
}
