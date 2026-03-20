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
            {
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, 2 }
                },
            }
        };

        initialGameState.World.InitializeLevel(1);

        var worldSnapshot = initialGameState.World.DeepClone();
        worldSnapshot.Monsters.ForEach(mp => {
            mp.Monster.Health = 3;
        });
        worldSnapshot.HeroActionPoints[SkillType.Movement] = 10;
        worldSnapshot.HeroActionPoints[SkillType.Attack] = 11;
        worldSnapshot.HeroActionPoints[SkillType.Defence] = 12;

        initialGameState.WorldSnapshot = worldSnapshot;

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
        var heroInitialX = 2;
        var heroInitialY = 4;
        var heroInitialMovementPoints = 3 + RandomUtility.RandomInt(0, 3);

        // set hero position to (2,3) no walls/monsters in surrounding spaces
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.World.HeroActionPoints.Add(SkillType.Movement, heroInitialMovementPoints);

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

        var actualHeroMovementPointsDelta = newGameState.World.HeroActionPoints[SkillType.Movement] - heroInitialMovementPoints;
        Assert.That(actualHeroMovementPointsDelta, Is.EqualTo(expectedMovementPointsDelta));
    }

    // orthogonal
    [TestCase(1, 0, 1)]
    [TestCase(1, 0, 0)]
    // diagonal
    [TestCase(1, 1, 2)]
    [TestCase(1, 1, 1)]
    [TestCase(1, 1, 0)]
    public void Movement_GivenNotEnoughMovement_WhenMoveHero_ThenDoNotMove_AndDoNotDecreaseMovementPoints(int xDelta, int yDelta, int initialMovementPoints)
    {
        var heroInitialX = 2;
        var heroInitialY = 4;
        
        // set hero position to (2,3) no walls/monsters in surrounding spaces
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.World.HeroActionPoints.Add(SkillType.Movement, initialMovementPoints);

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

        var actualHeroMovementPoints = newGameState.World.HeroActionPoints[SkillType.Movement];
        Assert.That(actualHeroMovementPoints, Is.EqualTo(initialMovementPoints));

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
        var heroInitialX = 3;
        var heroInitialY = 3;
        var initialMovementPoints = 10;
        
        // set hero position to center of level
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.World.HeroActionPoints.Add(SkillType.Movement, initialMovementPoints);

        // remove walls and monsters
        gameState.World.Walls.Clear();
        gameState.World.Monsters.Clear();

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

        var actualHeroMovementPoints = newGameState.World.HeroActionPoints[SkillType.Movement];
        Assert.That(actualHeroMovementPoints, Is.EqualTo(initialMovementPoints));

        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.CanOnlyMoveAdjacently));  
    }

    [TestCase(0, -1)]
    [TestCase(0, 1)]
    [TestCase(1, 1)]
    public void Movement_WhenMoveHeroIntoWallOrMonster_ThenDoNotMove_AndDoNotDecreaseMovementPoints(int xDelta, int yDelta)
    {
        var heroInitialX = 4;
        var heroInitialY = 3;
        var initialMovementPoints = 3;
        
        // set hero position to center of level
        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        // set hero movement points
        gameState.World.HeroActionPoints.Add(SkillType.Movement, initialMovementPoints);

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

        var actualHeroMovementPoints = newGameState.World.HeroActionPoints[SkillType.Movement];
        Assert.That(actualHeroMovementPoints, Is.EqualTo(initialMovementPoints));

        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.CannotMoveToThatSpace)); 
    }
    #endregion

    #region Attacking
    [Test]
    public void Attacking_GivenMonsterInRangeInLineOfSight_AndHeroHasEnoughAttackPoints_WhenAttack_ThenMonsterLosesHealth()
    {
        var heroInitialX = 5;
        var heroInitialY = 3;
        var initialAttackPoints = 4;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY);
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroPosition.X, Is.EqualTo(heroInitialX));
        Assert.That(newGameState.World.HeroPosition.Y, Is.EqualTo(heroInitialY));

        var monster = newGameState.World.Monsters.First(x => x.Position.X == monsterX && x.Position.Y == monsterY).Monster;
        Assert.That(monster.Health, Is.EqualTo(1)); // decreased from 2
        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(0));
    }

    [Test]
    public void Attacking_WhenAttackMonster_AndMonsterDefeated_RemoveMonsterFromWorld()
    {
        var heroInitialX = 5;
        var heroInitialY = 3;
        var initialAttackPoints = 4;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);
        gameState.World.Monsters.First(x => x.Position.X == monsterX && x.Position.Y == monsterY).Monster.Health = 1;

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroPosition.X, Is.EqualTo(heroInitialX));
        Assert.That(newGameState.World.HeroPosition.Y, Is.EqualTo(heroInitialY));

        var monsterPosition = newGameState.World.Monsters.FirstOrDefault(x => x.Position.X == monsterX && x.Position.Y == monsterY);
        Assert.That(monsterPosition, Is.Null);
        Assert.That(newGameState.World.Monsters.Count, Is.EqualTo(1)); // 1 other monster remaining
        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(0));
    }

    [Test]
    public void Attacking_GivenOneMonsterRemaining_WhenAttackMonster_AndMonsterDefeated_MoveToLevelEnd()
    {
        var heroInitialX = 5;
        var heroInitialY = 3;
        var initialAttackPoints = 4;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);
        gameState.World.Monsters.First(x => x.Position.X == monsterX && x.Position.Y == monsterY).Monster.Health = 1;
        var otherMonster = gameState.World.Monsters.First(x => x.Position.X == 4 && x.Position.Y == 5);
        gameState.World.Monsters.Remove(otherMonster);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.LevelEnd));
    }

    [Test]
    public void Attacking_GivenMonsterNotInRange_WhenAttack_ThenReturnGameMessage()
    {
        var heroInitialX = 5;
        var heroInitialY = 2;
        var initialAttackPoints = 4;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(initialAttackPoints));
        Assert.That(newGameState.World.Monsters.All(mp => mp.Monster.Health == 2), Is.True);
        Assert.That(newGameState.World.Monsters.Count, Is.EqualTo(2));
        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.MonsterNotInRangeToAttack));   
    }

    [Test]
    public void Attacking_GivenMonsterNotPresent_WhenAttack_ThenReturnGameMessage()
    {
        var heroInitialX = 5;
        var heroInitialY = 3;
        var initialAttackPoints = 4;
        
        var monsterX = 4;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(initialAttackPoints));
        Assert.That(newGameState.World.Monsters.All(mp => mp.Monster.Health == 2), Is.True);
        Assert.That(newGameState.World.Monsters.Count, Is.EqualTo(2));
        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.NoMonsterToAttackAtThatSpace));   
    }

    [TestCase(3,4)]
    [TestCase(2,4)]
    [TestCase(3,1)]
    [TestCase(1,1)]
    [TestCase(2,1)]
    public void Attacking_GivenMonsterNotInLineOfSight_WhenAttack_ThenReturnGameMessage(int heroInitialX, int heroInitialY)
    {
        var initialAttackPoints = 4;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY); 
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);
        gameState.Hero.Stats[SkillType.AttackRange] = 100;

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(initialAttackPoints));
        Assert.That(newGameState.World.Monsters.All(mp => mp.Monster.Health == 2), Is.True);
        Assert.That(newGameState.World.Monsters.Count, Is.EqualTo(2));
        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.MonsterNotInLineOfSightToAttack));
    }

    [Test]
    public void Attacking_GivenMonsterHasMoreDefenceThanYourAttack_WhenAttack_ThenReturnGameMessage()
    {
        var heroInitialX = 5;
        var heroInitialY = 3;
        var initialAttackPoints = 3;
        
        var monsterX = 5;
        var monsterY = 4;

        var gameState = _sut.GetCurrentState();
        gameState.World.HeroPosition = new Position(heroInitialX, heroInitialY);
        gameState.World.HeroActionPoints.Add(SkillType.Attack, initialAttackPoints);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(gameState));

        var inputEventParameters = new HeroActionAttackEventParameters()
        {
            X = monsterX,
            Y = monsterY
        };

        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionAttack,
            EventParameters = inputEventParameters
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(initialAttackPoints));
        Assert.That(newGameState.World.Monsters.All(mp => mp.Monster.Health == 2), Is.True);
        Assert.That(newGameState.World.Monsters.Count, Is.EqualTo(2));
        Assert.That(newGameState.GameMessage, Is.EqualTo(GameMessages.NotEnoughAttackToAttackMonster));   
    }
    #endregion

    #region Reset
    [Test]
    public void Reset_GivenHeroActionsMade_WhenReset_ThenReturnToStateAtStartOfActions()
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionReset
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.World.HeroActionPoints[SkillType.Movement], Is.EqualTo(10));
        Assert.That(newGameState.World.HeroActionPoints[SkillType.Attack], Is.EqualTo(11));
        Assert.That(newGameState.World.HeroActionPoints[SkillType.Defence], Is.EqualTo(12));
        Assert.That(newGameState.World.Monsters.Count(), Is.EqualTo(2));
        Assert.That(newGameState.World.Monsters.All(mp => mp.Monster.Health == 3), Is.True);
    }
    #endregion

    #region Continue
    [Test]
    public void Continue_WhenContinue_ThenGoToNextPhase()
    {
        _sut.ProcessInput(new InputEvent()
        {
            EventType = InputEventType.HeroActionEnd
        });

        var newGameState = _sut.GetCurrentState();

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
    }
    #endregion
}
