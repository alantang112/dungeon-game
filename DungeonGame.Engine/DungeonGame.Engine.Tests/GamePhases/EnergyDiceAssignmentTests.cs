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
            Hero = new Engine.Models.Entities.Hero()
            {
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 2 },
                    { SkillType.Defence, 4 },
                    { SkillType.AttackRange, 8 }
                }
            },
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

    [Test]
    public void WhenAssignLastDice_ThenProceedToHeroActions()
    {
        var gameState1 = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = 0,
                SkillType = SkillType.Movement
            }
        });

        Assert.That(gameState1.GamePhase, Is.EqualTo(GamePhase.EnergyDiceAssignment));

        var gameState2 = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = 1,
                SkillType = SkillType.Attack
            }
        });

        Assert.That(gameState2.GamePhase, Is.EqualTo(GamePhase.EnergyDiceAssignment));

        var gameState3 = _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.EnergyDiceAssign,
            EventParameters = new EnergyDiceAssignEventParameters()
            {
                DiceIndex = 2,
                SkillType = SkillType.Defence
            }
        });

        Assert.That(gameState3.GamePhase, Is.EqualTo(GamePhase.HeroActions));

        Assert.That(gameState3.World.HeroActionPoints[SkillType.Movement], Is.EqualTo(1 + 1));
        Assert.That(gameState3.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(2 + 4));
        Assert.That(gameState3.World.HeroActionPoints[SkillType.Defence], Is.EqualTo(4 + 6));
        Assert.That(gameState3.World.HeroActionPoints.Count(), Is.EqualTo(3));
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
}
