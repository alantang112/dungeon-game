using System.Text.Json;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

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
            GamePhase = GamePhase.HeroActions,
            EnergyDice = new EnergyDice()
            {
                Dice = new int[3] { 1, 4, 6 },
                AssignedSkills = new SkillType?[3] { null, null, null }
            },
            LevelNumber = 1,
            World = new World(),
            Hero = new Hero()
        };

        initialGameState.World.InitializeLevel(1);

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState);

        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }

    #region Movement
    [TestCase(1, 0, -2)]
    [TestCase(1, 1, -3)]
    [TestCase(0, 1, -2)]
    [TestCase(-1, 1, -3)]
    [TestCase(-1, 0, -2)]
    [TestCase(-1, -1, -3)]
    [TestCase(0, -1, -2)]
    [TestCase(1, -1, -3)]
    public void Movement_WhenMoveHero_ThenHeroMove_AndMovementPointsDecrease(int xDelta, int yDelta, int expectedMovementPointsDelta)
    {
        var heroInitialX = 1;
        var heroInitialY = 3;
        var heroInitialMovementPoints = 3 + RandomUtility.RandomInt(0, 3);

        // set hero position to (1,3) no walls/monsters in surrounding spaces
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.Hero.ActionPoints.Add(SkillType.Movement, heroInitialMovementPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionMoveEventParameters()
        {
            X = heroInitialX + xDelta,
            Y = heroInitialY + yDelta
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionMove,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();
        Assert.That(newGameState.World.HeroPosition.X, Is.EqualTo(inputEventParameters.X));
        Assert.That(newGameState.World.HeroPosition.Y, Is.EqualTo(inputEventParameters.Y));

        var actualHeroMovementPointsDelta = newGameState.Hero.ActionPoints[SkillType.Movement] - heroInitialMovementPoints;
        Assert.That(actualHeroMovementPointsDelta, Is.EqualTo(expectedMovementPointsDelta));
    }

    // orthogonal
    [TestCase(1, 0, 1)]
    [TestCase(1, 0, 0)]
    // diagonal
    [TestCase(1, 1, 2)]
    [TestCase(1, 1, 1)]
    [TestCase(1, 1, 0)]
    public void Movement_GivenNotEnoughMovement_WhenMoveHero_ThenDoNotMove_AndDoNotDecreaseMovementPoints(int xDelta, int yDelta, int initialMovementsPoints)
    {
        var heroInitialX = 1;
        var heroInitialY = 3;
        
        // set hero position to (1,3) no walls/monsters in surrounding spaces
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.Hero.ActionPoints.Add(SkillType.Movement, initialMovementsPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionMoveEventParameters()
        {
            X = heroInitialX + xDelta,
            Y = heroInitialY + yDelta
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionMove,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();
        Assert.That(newGameState.World.HeroPosition.X, Is.EqualTo(heroInitialX));
        Assert.That(newGameState.World.HeroPosition.Y, Is.EqualTo(heroInitialY));

        var actualHeroMovementPoints = newGameState.Hero.ActionPoints[SkillType.Movement];
        Assert.That(actualHeroMovementPoints, Is.EqualTo(initialMovementsPoints));

        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.NotEnoughMovementActionPoints));
    }

    [TestCase(0, 0)]
    [TestCase(-2, 0)]
    [TestCase(2, 0)]
    [TestCase(0, 2)]
    [TestCase(0, -2)]
    [TestCase(-2, 1)]
    [TestCase(2, 1)]
    [TestCase(1, 2)]
    [TestCase(1, -2)]
    public void Movement_WhenMoveHeroNotAdjacent_ThenDoNotMove_AndDoNotDecreaseMovementPoints(int xDelta, int yDelta)
    {
        throw new NotImplementedException();  
    }

    [Test]
    public void Movement_WhenMoveHeroIntoWall_ThenDoNotMove_AndDoNotDecreaseMovementPoints()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Movement_WhenMoveHeroIntoSameSpace_ThenDoNotMove_AndDoNotDecreaseMovementPoints()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Movement_WhenMoveHeroIntoMonster_ThenDoNotMove_AndDoNotDecreaseMovementPoints_AndReturnGameError()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Attacking
    [Test]
    public void Attacking_GivenMonsterInRangeInLineOfSight_AndHeroHasEnoughAttackPoints_WhenAttack_ThenMonsterLosesHealth()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Attacking_GivenMonsterNotInRange_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Attacking_GivenMonsterNotPresent_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Attacking_GivenMonsterNotInLineOfSight_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Attacking_GivenMonsterHasMoreDefenceThanYourAttack_WhenAttack_ThenReturnGameMessage()
    {
        throw new NotImplementedException();
    }

    /*
    * TODO: need another unit test class to test InRange, InLineOfSight
    */

    #endregion

    #region Continue
    [Test]
    public void Continue_WhenContinue_ThenGoToNextPhase()
    {
        throw new NotImplementedException();
    }
    #endregion
}
