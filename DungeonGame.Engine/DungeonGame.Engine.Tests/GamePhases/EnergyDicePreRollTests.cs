using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

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
        }, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameState);
    }

    [Test]
    public void WhenRoll_ThenInitializeEnergyDice()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        Assert.That(gameState.EnergyDice.Dice.All(x => x >= 1 && x <= 6), Is.True);
    }

    [Test]
    public void WhenRoll_ThenResetAssignment()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.EnergyDice.AssignDice(0, SkillType.Movement);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        Assert.That(gameState.EnergyDice.AssignedSkills.All(x => x == null), Is.True);
    }

    [Test]
    public void WhenRoll_ThenUpdateGamePhaseToEnergyDiceAssignment()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.EnergyDiceAssignment));
    }
}
