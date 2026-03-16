using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

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
        });

        _sut.LoadGameStateSnapshot(initialGameState);
    }
    
    [TestCase(0, SkillType.Movement)]
    [TestCase(1, SkillType.Attack)]
    [TestCase(2, SkillType.Defence)]
    public void WhenAssignDiceToSkill_ThenAssignDice(int index, SkillType skillType)
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = skillType
            }
        });

        var gameState = _sut.GetCurrentState();

        Assert.That(gameState.EnergyDice.AssignedSkills[index], Is.EqualTo(skillType));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void GivenDiceAssigned_WhenAssignDiceToAlreadyAssignedSkill_ThenReturnInvalid(int index)
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = SkillType.Movement
            }
        });

        var gameState = _sut.GetCurrentState();
        Assert.That(gameState.GameMessage, Is.Not.EqualTo(GameConstants.SkillAlreadyAssignedEnergyDice));

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = index,
                SkillType = SkillType.Movement
            }
        });

        var newGameState = _sut.GetCurrentState();
        Assert.That(newGameState.GameMessage, Is.EqualTo(GameConstants.SkillAlreadyAssignedEnergyDice));
    }

    [TestCase(SkillType.AttackRange)]
    public void WhenAssignDiceToInvalidSkill_ThenReturnInvalid(SkillType skillType)
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = 0,
                SkillType = skillType
            }
        });

        var gameState = _sut.GetCurrentState();
        Assert.That(gameState.GameMessage, Is.EqualTo(GameConstants.InvalidSkillForEnergyDiceAssignment));
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
        });

        _sut.LoadGameStateSnapshot(initialGameState);

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceResetAssignment
        });

        var gameState = _sut.GetCurrentState();

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
        });

        _sut.LoadGameStateSnapshot(initialGameState);

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceConfirmAssignment
        });

        var gameState = _sut.GetCurrentState();
        
        Assert.That(gameState.GameMessage, Is.EqualTo(GameConstants.AssignAllEnergyDiceBeforeProceeding));
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
            }
        });

        _sut.LoadGameStateSnapshot(initialGameState);

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceConfirmAssignment
        });

        var gameState = _sut.GetCurrentState();
        
        Assert.That(gameState.GamePhase, Is.EqualTo(GamePhase.HeroActions));
    }
}
