using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class EnergyDiceAssignmentTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
        
        var initialGameState = JsonSerializer.Serialize(new GameState()
        {
            GamePhase = GamePhase.EnergyDiceAssignment,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { null, null, null }
            }
        }, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameState);
    }
    
    [TestCase(0, SkillType.Movement)]
    [TestCase(1, SkillType.Attack)]
    [TestCase(2, SkillType.Defence)]
    public void WhenAssignDiceToSkill_ThenAssignDice(int index, SkillType skillType)
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = skillType
            }
        });

        Assert.That(gameState.EnergyDice.AssignedSkills[index], Is.EqualTo(skillType));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void GivenDiceAssigned_WhenAssignDiceToAlreadyAssignedSkill_ThenReturnInvalid(int index)
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = SkillType.Movement
            }
        });

        Assert.That(gameState.GameMessage, Is.Not.EqualTo(GameMessages.SkillAlreadyAssignedEnergyDice));

        var newGameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = SkillType.Movement
            }
        });

        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.SkillAlreadyAssignedEnergyDice));
    }

    [TestCase(SkillType.AttackRange)]
    public void WhenAssignDiceToInvalidSkill_ThenReturnInvalid(SkillType skillType)
    {
        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = 0,
                SkillType = skillType
            }
        });

        Assert.That(gameState.GameMessage, Is.EqualTo(GameMessages.InvalidSkillForEnergyDiceAssignment));
    }

    [Test]
    public void WhenResetAssignment_ThenResetAssignment()
    {
        var initialGameState = JsonSerializer.Serialize(new GameState()
        {
            GamePhase = GamePhase.EnergyDiceAssignment,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { SkillType.Movement, SkillType.Defence, SkillType.Attack }
            }
        }, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameState);

        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceResetAssignment
        });

        Assert.That(gameState.EnergyDice.AssignedSkills.All(x => x == null), Is.True);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void GivenNotAllAssigned_WhenConfirm_ThenReturnInvalid(int assignedCount)
    {
        var energyDice = new EnergyDice()
        {
            Dice = new int[3] { 1, 4, 6 },
            AssignedSkills = new SkillType?[3] { SkillType.Movement, SkillType.Defence, SkillType.Attack }
        };

        switch (assignedCount)
        {
            case 0:
                energyDice.AssignedSkills = new SkillType?[3];
                break;
            case 1:
                energyDice.AssignedSkills[1] = null;
                energyDice.AssignedSkills[2] = null;
                break;
            case 2:
                energyDice.AssignedSkills[1] = null;
                break;
            default:
                throw new NotImplementedException();
        }

        var initialGameState = JsonSerializer.Serialize(new GameState()
        {
            GamePhase = GamePhase.EnergyDiceAssignment,
            EnergyDice = energyDice
        }, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameState);

        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceConfirmAssignment
        });
        
        Assert.That(gameState.GameMessage, Is.EqualTo(GameMessages.AssignAllEnergyDiceBeforeProceeding));
    }

    [Test]
    public void GivenAllAssigned_WhenConfirm_ThenMoveToNextGamePhase()
    {
        var initialGameState = JsonSerializer.Serialize(new GameState()
        {
            GamePhase = GamePhase.EnergyDiceAssignment,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { SkillType.Movement, SkillType.Defence, SkillType.Attack }
            },
            Hero = new Engine.Models.Entities.Hero()
            {
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 2 },
                    { SkillType.Defence, 3 },
                    { SkillType.AttackRange, 4 }
                }
            }
        }, SerializationUtility.JsonSerializerOptions);

        _sut.LoadGameStateSnapshot(initialGameState);

        var gameState = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceConfirmAssignment
        });
        
        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.HeroActions));

        Assert.That(gameState.World.HeroActionPoints[SkillType.Movement], Is.EqualTo(2));
        Assert.That(gameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(8));
        Assert.That(gameState.World.HeroActionPoints[SkillType.Defence], Is.EqualTo(7));
        Assert.That(gameState.World.HeroActionPoints.Count(), Is.EqualTo(3));

        Assert.That(gameState.WorldSnapshot, Is.Not.Null);
        Assert.That(gameState.WorldSnapshot.HeroActionPoints[SkillType.Movement], Is.EqualTo(2));
        Assert.That(gameState.WorldSnapshot.HeroActionPoints[SkillType.Attack], Is.EqualTo(8));
        Assert.That(gameState.WorldSnapshot.HeroActionPoints[SkillType.Defence], Is.EqualTo(7));
        Assert.That(gameState.WorldSnapshot.HeroActionPoints.Count(), Is.EqualTo(3));
    }
}
