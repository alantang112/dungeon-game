using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;

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
        });

        _sut.LoadGameStateSnapshot(initialGameState);
    }
    
    [TestCase(0, SkillType.Movement)]
    [TestCase(1, SkillType.Attack)]
    [TestCase(2, SkillType.Defence)]
    public void WhenAssignDiceToSkill_ThenAssignDice(int index, SkillType skillType)
    {
        
    }

    [Test]
    public void GivenDiceAssigned_WhenAssignDiceToSkill_ThenAssignDice()
    {
        
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void GivenDiceAssigned_WhenAssignDiceToAlreadyAssignedSkill_ThenReturnInvalid(int index)
    {
        
    }

    [TestCase(SkillType.AttackRange)]
    public void WhenAssignDiceToInvalidSkill_ThenReturnInvalid(SkillType skillType)
    {
        
    }

    [Test]
    public void GivenAllAssigned_WhenAssignDice_ThenReturnInvalid()
    {
        
    }

    [Test]
    public void WhenResetAssignment_ThenResetAssignment()
    {
        
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void GivenNotAllAssigned_WhenConfirm_ThenReturnInvalid(int assignedCount)
    {
        
    }

    [Test]
    public void GivenAllAssigned_WhenConfirm_ThenMoveToNextGamePhase()
    {
        
    }
}
