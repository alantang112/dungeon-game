using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Tests.GamePhases;

public class HeroActionTests
{
    private IGameEngine _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new GameEngine();
        
        var initialGameState = new GameState()
        {
            GamePhase = GamePhase.EnergyDiceAssignment,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { null, null, null }
            },
            LevelNumber = 1,
            World = new World()
        };

        initialGameState.World.InitializeLevel(1);

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState);

        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }

    #region Movement
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(0, 1)]
    [TestCase(-1, 1)]
    [TestCase(-1, 0)]
    [TestCase(-1, -1)]
    [TestCase(0, -1)]
    [TestCase(1, -1)]
    public void WhenMoveHero_ThenHeroMove(int xDelta, int yDelta)
    {
        throw new NotImplementedException();    
    }

    // orthogonal
    [TestCase(1, 1, 0)]
    [TestCase(0, 1, 0)]
    // diagonal
    [TestCase(2, 1, 1)]
    [TestCase(1, 1, 1)]
    [TestCase(0, 1, 1)]
    public void GivenNotEnoughMovement_WhenMoveHero_ThenDoNotMove(int movementsRemaining, int xDelta, int yDelta)
    {
        throw new NotImplementedException();  
    }

    [TestCase(0, 0)]
    [TestCase(-2, 0)]
    [TestCase(2, 0)]
    [TestCase(0, 2)]
    [TestCase(0, -2)]
    public void WhenMoveHeroNotAdjacent_ThenDoNotMove(int xDelta, int yDelta)
    {
        throw new NotImplementedException();  
    }

    [Test]
    public void WhenMoveHeroIntoWall_ThenDoNotMove()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void WhenMoveHeroIntoMonster_ThenDoNotMove_AndReturnGameError()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Attacking
    [Test]
    public void GivenMonsterInRangeInLineOfSight_AndHeroHasEnoughAttackPoints_WhenAttack_ThenMonsterLosesHealth()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GivenMonsterNotInRange_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GivenMonsterNotPresent_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GivenMonsterNotInLineOfSight_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GivenMonsterHasMoreDefenceThanYourAttack_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }
    
    /*
    * TODO: need another unit test class to test InRange, InLineOfSight
    */

    #endregion

    #region Continue
    [Test]
    public void WhenContinue_ThenGoToNextPhase()
    {
        throw new NotImplementedException();
    }
    #endregion
}
