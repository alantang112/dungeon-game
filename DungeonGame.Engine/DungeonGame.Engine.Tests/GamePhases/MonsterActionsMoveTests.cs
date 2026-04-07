using System.Text.Json;
using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Tests.GamePhases;

public class MonsterActionsMoveTests
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
            World = new World()
            {
                HeroActionPoints = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 }
                }
            },
            Hero = new Hero()
            {
                Health = 10,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 1 },
                    { SkillType.Attack, 1 },
                    { SkillType.Defence, 1 },
                    { SkillType.AttackRange, 2 }
                },
            }
        };

        initialGameState.World.InitializeLevel(1, initRandomWalls: false);

        initialGameState.World.Monsters.RemoveAt(1);

        initialGameState.World.HeroActionPoints = new Dictionary<SkillType, int>()
        {
            { SkillType.Movement, 0 },
            { SkillType.Attack, 0 },
            { SkillType.Defence, 7 },
        };

        var initialGameStateJson = JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions);
        _sut.LoadGameStateSnapshot(initialGameStateJson);
    }   

    [Test]
    public void GivenMonsterAlreadyAtMaxAttackRange_AndInLineOfSight_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 4);
        initialGameState.World.Monsters[0].Position = new Position(4, 3);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 3)));
        Assert.That(newGameState.World.Monsters[0].LastMovementPath, Is.Empty);
    }

    [Test]
    public void GivenMonsterHasNoWalkableSquares_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(4, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(5, 5)));
        Assert.That(newGameState.World.Monsters[0].LastMovementPath, Is.Empty);
    }

    [Test]
    public void GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_ButCurrentSquareIsBetter_ThenDoNotMove()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(3, 5);
        initialGameState.World.Monsters[0].Position = new Position(5, 5);
        initialGameState.World.Monsters[0].Monster.Stats[SkillType.AttackRange] = 4;
        initialGameState.World.Walls.Add(new Position(3, 4));
        initialGameState.World.Walls.Add(new Position(5, 4));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(5, 5)));
        Assert.That(newGameState.World.Monsters[0].LastMovementPath, Is.Empty);
    }

    [TestCase(2, 1, 5, 3, null, null, 3, 2)]
    [TestCase(3, 4, 2, 4, null, null, 2, 3)] // (2,5) is also acceptable
    [TestCase(1, 1, 3, 3, 3, 1, 1, 2)]
    public void GivenWalkableSquaresIsInAttackRangeAndInLineOfSight_AndIsBetterThanCurrentSquare_ThenMove(int heroX, int heroY, int monsterX, int monsterY, int? extraWallX, int? extraWallY, int expectedMonsterX, int expectedMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        if (extraWallX.HasValue)
        {
            initialGameState.World.Walls.Add(new Position(extraWallX.Value, extraWallY!.Value));
        }

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
        Assert.That(newGameState.World.Monsters[0].LastMovementPath, Is.Not.Empty);
    }

    [TestCase(2, 1, 5, 5, 4, 3)]
    [TestCase(4, 1, 5, 4, 5, 2)]
    [TestCase(1, 3, 4, 5, 2, 4)]
    public void GivenNoWalkableSquaresInAttackRangeAndLineOfSight_ThenMoveToClosestSquareAtMaxAttackRangeAndLineSight(int heroX, int heroY, int monsterX, int monsterY, int expectedMonsterX, int expectedMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
        Assert.That(newGameState.World.Monsters[0].LastMovementPath, Is.Not.Empty);
    }

    [TestCase(2, 1, 5, 5, 4, 5, null, null, null, null, 4, 3, 3, 3)]
    [TestCase(1, 1, 3, 2, 5, 3, 1, 3,  null, null, 2, 1, 3, 2)] 
    [TestCase(1, 1, 2, 3, 1, 3, 2, 1,  null, null, 1, 3, 1, 2)]
    [TestCase(2, 1, 3, 1, 3, 3, 3, 2,  null, null, 3, 1, 1, 2)]
    [TestCase(1, 1, 4, 5, 5, 4, 3, 3, 5, 2, 2, 4, 3, 5)]
    [TestCase(3, 1, 2, 1, 4, 3, 3, 2, 4, 1, 2, 1, 2, 3)] // move as close as possible to hero
    [TestCase(3,2, 3,3, 2,4, 5,1, 2,3, 4,3, 3,3)]
    public void GivenTwoMonsters_NoWalkableSquaresInAttackRangeAndLineOfSight_ThenMoveToClosestSquareAtMaxAttackRangeAndLineSight(int heroX, int heroY, int monsterX, int monsterY, 
        int otherMonsterX, int otherMonsterY, int? extraWallX, int? extraWallY, int? extraWall2X, int? extraWall2Y, int expectedMonsterX, int expectedMonsterY, int expectedOtherMonsterX, int expectedOtherMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(heroX, heroY);
        initialGameState.World.Monsters[0].Position = new Position(monsterX, monsterY);

        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = initialGameState.World.Monsters[0].Monster,
            Position = new Position(otherMonsterX, otherMonsterY)
        });
        initialGameState.World.Monsters[1].Monster.Id = Guid.NewGuid();

        if (extraWallX.HasValue)
        {
            initialGameState.World.Walls.Add(new Position(extraWallX.Value, extraWallY!.Value));
        }

        if (extraWall2X.HasValue)
        {
            initialGameState.World.Walls.Add(new Position(extraWall2X.Value, extraWall2Y!.Value));
        }

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
        Assert.That(newGameState.World.Monsters[1].Position, Is.EqualTo(new Position(expectedOtherMonsterX, expectedOtherMonsterY)));
        var expectPathIsNotEmpty = monsterX != expectedMonsterX || monsterY != expectedMonsterY;
        Assert.That(newGameState.World.Monsters[0].LastMovementPath.Count() > 0, Is.EqualTo(expectPathIsNotEmpty));
        var expectOtherPathIsNotEmpty = otherMonsterX != expectedOtherMonsterX || otherMonsterY != expectedOtherMonsterY;
        Assert.That(newGameState.World.Monsters[1].LastMovementPath.Count() > 0, Is.EqualTo(expectOtherPathIsNotEmpty));
    }

    [Test]
    public void GivenMonsterMovementTargetOutsideOfWalkableRange_MoveIntoLessIdealPosition()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.InitializeLevel(-1);

        initialGameState.World.HeroPosition = new Position(4, 4);
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Spider),
            Position = new Position(5, 5)
        });
        initialGameState.World.Monsters[0].Monster.Stats = new Dictionary<SkillType, int>()
        {
            { SkillType.Movement, 6 },
            { SkillType.Attack, 6 },
            { SkillType.Defence, 6 },
            { SkillType.AttackRange, 4 }
        };

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(3, 4)));
    }

    [Test]
    public void GivenWallIsland_PathInTheCorrectDirection()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.InitializeLevel(-1);

        initialGameState.World.HeroPosition = new Position(1, 2);
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Skeleton),
            Position = new Position(3, 5)
        });
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Skeleton),
            Position = new Position(4, 4)
        });
        initialGameState.World.Walls.Add(new Position(2, 3));
        initialGameState.World.Walls.Add(new Position(2, 4));
        initialGameState.World.Walls.Add(new Position(3, 4));
        initialGameState.World.Walls.Add(new Position(4, 2));
        initialGameState.World.Walls.Add(new Position(4, 3));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        // Note: They go this way because the ideal position is at (3, 1)
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(4, 5)));
        Assert.That(newGameState.World.Monsters[1].Position, Is.EqualTo(new Position(5, 4)));
    }

    [Test]
    public void GivenTwoPathsToIdealPosition_PickBasedOnBetterRangeFromTarget()
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.InitializeLevel(-1);

        initialGameState.World.HeroPosition = new Position(1, 3);
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Skeleton),
            Position = new Position(1, 2)
        });
        initialGameState.World.Walls.Add(new Position(2, 3));

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(1, 1)));
    }

    [Test]
    public void GivenNoWalkableSquaresInAttackRangeAndLineOfSight_AndNoEmptySquaresInAttackRangeAndLineOfSight_ThenMoveAsCloseAsPossibleToHero()
    {
        var originalMonsterPosition = new Position(2, 5);

        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.HeroPosition = new Position(1, 1);
        initialGameState.World.Monsters[0].Position = originalMonsterPosition;
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = new Monster()
            {
                Health = 1,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 0 },
                    { SkillType.Attack, 2 },
                    { SkillType.Defence, 2 },
                    { SkillType.AttackRange, 2 }
                }
            },
            Position = new Position(2, 1)
        });
        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = new Monster()
            {
                Health = 1,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 0 },
                    { SkillType.Attack, 2 },
                    { SkillType.Defence, 2 },
                    { SkillType.AttackRange, 2 }
                }
            },
            Position = new Position(1, 2)
        });

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
        Assert.That(newGameState.World.Monsters[0].Position, Is.EqualTo(new Position(1, 3)));

        var expectedPaths = new List<List<Position>>()
        {
            new List<Position>() { new Position(2, 4), new Position(1, 3) },
            new List<Position>() { new Position(1, 4), new Position(1, 3) },
        };
        
        var actual = newGameState.World.Monsters[0].LastMovementPath;
        Assert.That(actual.Count, Is.EqualTo(3));
        Assert.That(actual[0], Is.EqualTo(originalMonsterPosition));
        Assert.That(expectedPaths.Any(e => e[0] == actual[1] && e[1] == actual[2]), Is.True);
    }

    [Test]
    public void GivenWalkPathGoesThroughMonster_ThenReturnMovementPath()
    {
        var gameStateJson = """{"GamePhase":"HeroActions","EnergyDice":{"Dice":[2,4,1],"AssignedSkills":["Attack","Defence","Movement"]},"EnergyDiceSnapshot":{"Dice":[2,4,1],"AssignedSkills":[null,null,null]},"Hero":{"Name":"Lil Pete","BirthYear":2026,"isMaleName":true,"Health":5,"Stats":{"Movement":1,"Attack":1,"Defence":1,"AttackRange":2}},"World":{"HeroPosition":{"X":3,"Y":1},"HeroActionPoints":{"Attack":3,"Defence":5,"Movement":0},"Borders":[{"X":0,"Y":0},{"X":6,"Y":0},{"X":0,"Y":6},{"X":1,"Y":0},{"X":0,"Y":1},{"X":6,"Y":1},{"X":1,"Y":6},{"X":2,"Y":0},{"X":0,"Y":2},{"X":6,"Y":2},{"X":2,"Y":6},{"X":3,"Y":0},{"X":0,"Y":3},{"X":6,"Y":3},{"X":3,"Y":6},{"X":4,"Y":0},{"X":0,"Y":4},{"X":6,"Y":4},{"X":4,"Y":6},{"X":5,"Y":0},{"X":0,"Y":5},{"X":6,"Y":5},{"X":5,"Y":6},{"X":6,"Y":6}],"Walls":[{"X":0,"Y":0},{"X":6,"Y":0},{"X":0,"Y":6},{"X":1,"Y":0},{"X":0,"Y":1},{"X":6,"Y":1},{"X":1,"Y":6},{"X":2,"Y":0},{"X":0,"Y":2},{"X":6,"Y":2},{"X":2,"Y":6},{"X":3,"Y":0},{"X":0,"Y":3},{"X":6,"Y":3},{"X":3,"Y":6},{"X":4,"Y":0},{"X":0,"Y":4},{"X":6,"Y":4},{"X":4,"Y":6},{"X":5,"Y":0},{"X":0,"Y":5},{"X":6,"Y":5},{"X":5,"Y":6},{"X":6,"Y":6},{"X":2,"Y":2},{"X":4,"Y":2},{"X":4,"Y":4},{"X":3,"Y":3},{"X":4,"Y":1}],"Monsters":[{"Monster":{"Type":"Spider","Name":"Moe","Health":2,"MaxHealth":2,"Stats":{"Movement":5,"Attack":4,"Defence":4,"AttackRange":3}},"Position":{"X":2,"Y":3},"LastMovementPath":[{"X":3,"Y":5},{"X":3,"Y":4},{"X":2,"Y":3}]},{"Monster":{"Type":"Spider","Name":"Zack","Health":2,"MaxHealth":2,"Stats":{"Movement":5,"Attack":4,"Defence":4,"AttackRange":3}},"Position":{"X":1,"Y":2},"LastMovementPath":[{"X":2,"Y":4},{"X":2,"Y":3},{"X":1,"Y":2}]}]},"WorldSnapshot":{"HeroPosition":{"X":2,"Y":1},"HeroActionPoints":{},"Borders":[{"X":0,"Y":0},{"X":6,"Y":0},{"X":0,"Y":6},{"X":1,"Y":0},{"X":0,"Y":1},{"X":6,"Y":1},{"X":1,"Y":6},{"X":2,"Y":0},{"X":0,"Y":2},{"X":6,"Y":2},{"X":2,"Y":6},{"X":3,"Y":0},{"X":0,"Y":3},{"X":6,"Y":3},{"X":3,"Y":6},{"X":4,"Y":0},{"X":0,"Y":4},{"X":6,"Y":4},{"X":4,"Y":6},{"X":5,"Y":0},{"X":0,"Y":5},{"X":6,"Y":5},{"X":5,"Y":6},{"X":6,"Y":6}],"Walls":[{"X":0,"Y":0},{"X":6,"Y":0},{"X":0,"Y":6},{"X":1,"Y":0},{"X":0,"Y":1},{"X":6,"Y":1},{"X":1,"Y":6},{"X":2,"Y":0},{"X":0,"Y":2},{"X":6,"Y":2},{"X":2,"Y":6},{"X":3,"Y":0},{"X":0,"Y":3},{"X":6,"Y":3},{"X":3,"Y":6},{"X":4,"Y":0},{"X":0,"Y":4},{"X":6,"Y":4},{"X":4,"Y":6},{"X":5,"Y":0},{"X":0,"Y":5},{"X":6,"Y":5},{"X":5,"Y":6},{"X":6,"Y":6},{"X":2,"Y":2},{"X":4,"Y":2},{"X":4,"Y":4},{"X":3,"Y":3},{"X":4,"Y":1}],"Monsters":[{"Monster":{"Type":"Spider","Name":"Moe","Health":2,"MaxHealth":2,"Stats":{"Movement":5,"Attack":4,"Defence":4,"AttackRange":3}},"Position":{"X":2,"Y":3},"LastMovementPath":[{"X":3,"Y":5},{"X":3,"Y":4},{"X":2,"Y":3}]},{"Monster":{"Type":"Spider","Name":"Zack","Health":2,"MaxHealth":2,"Stats":{"Movement":5,"Attack":4,"Defence":4,"AttackRange":3}},"Position":{"X":1,"Y":2},"LastMovementPath":[{"X":2,"Y":4},{"X":2,"Y":3},{"X":1,"Y":2}]}]},"LevelNumber":1,"GameMessage":"","ScheduledEvents":[],"GameMessageLog":[]}""";

        _sut.LoadGameStateSnapshot(gameStateJson);

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.GamePhase, Is.EqualTo(GamePhase.MonsterActions));
    }

    [TestCase(2,2, "2,3|3,3|4,3", "1,2", MonsterType.Minotaur, 1,3, 2,4)]
    [TestCase(2,2, "2,3|3,3|4,3", "1,2", MonsterType.Minotaur, 1,4, 2,4)]
    [TestCase(2,2, "2,3|3,3|4,3|4,4", "1,2", MonsterType.Fiendling, 1,3, 1,1)]
    [TestCase(2,2, "2,3|3,3|4,3|4,4", "1,2", MonsterType.Fiendling, 1,4, 1,3)]
    [TestCase(1,1, "2,3|3,3|4,3|4,4", "1,2|1,3", MonsterType.Fiendling, 2,4, 3,5)]
    [TestCase(1,1, "2,3|3,3|4,3|4,4", "1,2|1,3", MonsterType.Fiendling, 3,5, 5,5)]
    public void GivenMonstersInTheWay_ThenConsiderThatPathAsNotWalkable(int heroX, int heroY, string walls, string stationaryMonsters, MonsterType monsterType, int monsterX, int monsterY, int expectedMonsterX, int expectedMonsterY)
    {
        var initialGameState = _sut.GetCurrentState();

        initialGameState.World.InitializeLevel(-1);

        initialGameState.World.HeroPosition = new Position(heroX, heroY);

        foreach(var wall in walls.Split("|").Where(x => !string.IsNullOrEmpty(x)))
        {
            var wallSplit = wall.Split(",");
            var wallX = int.Parse(wallSplit[0]);
            var wallY = int.Parse(wallSplit[1]);

            initialGameState.World.Walls.Add(new Position(wallX, wallY));
        }
        
        foreach(var stationaryMonster in stationaryMonsters.Split("|").Where(x => !string.IsNullOrEmpty(x)))
        {
            var stationaryMonsterSplit = stationaryMonster.Split(",");
            var stationaryMonsterX = int.Parse(stationaryMonsterSplit[0]);
            var stationaryMonsterY = int.Parse(stationaryMonsterSplit[1]);

            var newMonster = MonsterSpawner.Spawn(MonsterType.Spider);
            newMonster.SetStat(SkillType.Movement, 0);

            initialGameState.World.Monsters.Add(new MonsterPosition()
            {
                Monster = newMonster,
                Position = new Position(stationaryMonsterX, stationaryMonsterY)
            });
        }

        initialGameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(monsterType),
            Position = new Position(monsterX, monsterY)
        });

        _sut.LoadGameStateSnapshot(JsonSerializer.Serialize(initialGameState, SerializationUtility.JsonSerializerOptions));

        var newGameState = _sut.ProcessInput(new Engine.Models.InputEventModels.InputEvent()
        {
            EventType = Engine.Models.Enums.InputEventType.HeroActionEnd
        });

        Assert.That(newGameState.World.Monsters.Last().Position, Is.EqualTo(new Position(expectedMonsterX, expectedMonsterY)));
    }
}
