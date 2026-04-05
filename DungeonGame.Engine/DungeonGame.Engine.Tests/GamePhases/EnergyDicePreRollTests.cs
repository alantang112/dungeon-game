using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;
using DungeonGame.Engine.GameTemplates;

namespace DungeonGame.Engine.Tests.GamePhases;

public class EnergyDicePreRollTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
        
        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.EnergyDicePreRoll,
            World = new World()
            {
                Monsters = new List<Engine.Models.Entities.MonsterPosition>()
                {
                    new Engine.Models.Entities.MonsterPosition()
                    {
                        Monster = MonsterSpawner.Spawn(MonsterType.Overseer),
                        Position = new Engine.Models.Geometry.Position(5, 5)
                    }
                }
            }
        };

        initialGameState.World.Monsters[0].Monster.BossDice = new Dictionary<SkillType, int>()
        {
            { SkillType.Movement, 1 },
            { SkillType.Attack, 2 },
            { SkillType.Defence, 3 },
        };

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }

    [Test]
    public void WhenRoll_ThenInitializeEnergyDice()
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceRoll
        });

        Assert.That(gameState.EnergyDice.Dice.All(x => x >= 1 && x <= 6), Is.True);

        // Boss Dice is not re-rolled
        Assert.That(gameState.World.Monsters[0].Monster.BossDice[SkillType.Movement], Is.EqualTo(1));
        Assert.That(gameState.World.Monsters[0].Monster.BossDice[SkillType.Attack], Is.EqualTo(2));
        Assert.That(gameState.World.Monsters[0].Monster.BossDice[SkillType.Defence], Is.EqualTo(3));
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
