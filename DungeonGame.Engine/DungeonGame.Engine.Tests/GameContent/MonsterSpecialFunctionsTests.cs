using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Tests.GameContent;

public class MonsterSpecialFunctionsTests
{
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 5, SkillType.Attack)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 4, SkillType.Defence)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 3, SkillType.AttackRange)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 2, SkillType.Movement)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 1, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 5, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 4, SkillType.Defence)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 3, SkillType.AttackRange)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 2, SkillType.Movement)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 1, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 0, null)]
    public void GivenColossusWithRandomSeed_ThenDeterministicallyLevelStat(string guid, int currentHealth, SkillType? expectedSkillTypeLeveled)
    {
        var monster = MonsterSpawner.Spawn(Engine.Models.Enums.MonsterType.Colossus);
        monster.Health = currentHealth;

        monster.RandomSeed = new Guid(guid);

        var initialStats = new Dictionary<SkillType, int>(monster.Stats);

        MonsterSpecialFunctions.PostDamageFunction(monster, new GameState());

        Assert.Multiple(() =>
        {
            foreach(var skillType in initialStats.Keys)
            {
                var initialValue = initialStats[skillType];
                var expected = expectedSkillTypeLeveled == skillType ? initialValue + 1 : initialValue;
                var actual = monster.Stats[skillType];
                Assert.That(actual, Is.EqualTo(expected), $"Expected {skillType} to be {expected} but was {actual}");
            }
        });
    }

    [TestCase(3,3, 3,4, 2)]
    [TestCase(3,3, 4,4, 2)]
    [TestCase(3,3, 5,3, 2)]
    [TestCase(3,3, 5,4, 1)]
    public void GivenDirewolves_WhenPostMove_RecalculateState(int posX, int posY, int pos2X, int pos2Y, int expectedPhase)
    {
        var gameState = new GameState();

        gameState.World.InitializeLevel(-1);

        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(posX, posY)
        });
        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(pos2X, pos2Y)
        });

        MonsterSpecialFunctions.PostMonstersMoveFunction(gameState);

        var expectedAttack = GameConstants.DirewolfBaseAttack + (expectedPhase == 2 ? GameConstants.DirewolfBonusAttack : 0);

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Monsters[1].Monster.Phase, Is.EqualTo(expectedPhase));

        Assert.That(gameState.World.Monsters[0].Monster.GetStat(SkillType.Attack), Is.EqualTo(expectedAttack));
        Assert.That(gameState.World.Monsters[1].Monster.GetStat(SkillType.Attack), Is.EqualTo(expectedAttack));
    }

    public void GivenThreeDirewolves_WhenPostMove_RecalculateState()
    {
        var gameState = new GameState();

        gameState.World.InitializeLevel(-1);

        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(3, 3)
        });
        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(3, 4)
        });
        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(3, 5)
        });

        MonsterSpecialFunctions.PostMonstersMoveFunction(gameState);

        var expectedAttack = GameConstants.DirewolfBaseAttack + GameConstants.DirewolfBonusAttack * 2;

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(3));
        Assert.That(gameState.World.Monsters[1].Monster.Phase, Is.EqualTo(3));

        Assert.That(gameState.World.Monsters[0].Monster.GetStat(SkillType.Attack), Is.EqualTo(expectedAttack));
        Assert.That(gameState.World.Monsters[1].Monster.GetStat(SkillType.Attack), Is.EqualTo(expectedAttack));
    }

    [Test]
    public void GivenDireWolves_WhenPostHeroAttack_AndDirewolfDefeated_RecalculateState()
    {
        var gameState = new GameState();

        gameState.World.InitializeLevel(-1);

        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(1, 1)
        });
        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Direwolf),
            Position = new Engine.Models.Geometry.Position(1, 2)
        });

        gameState.World.Monsters[0].Monster.Health = 0;

        MonsterSpecialFunctions.PostDamageFunction(gameState.World.Monsters[0].Monster, gameState);

        Assert.That(gameState.World.Monsters[1].Monster.Phase, Is.EqualTo(1));
        Assert.That(gameState.World.Monsters[1].Monster.GetStat(SkillType.Attack), Is.EqualTo(GameConstants.DirewolfBaseAttack));
    }

    [TestCase(1, true, 1)]
    [TestCase(1, false, -1)]
    [TestCase(2, true, -2)]
    [TestCase(2, false, 2)]
    public void GivenReaper_AndMoved_WhenPostMonstersAttack_RecalculateState(int initialPhase, bool moved, int expectedPhase)
    {
        var gameState = new GameState();

        gameState.World.InitializeLevel(-1);

        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Reaper),
            Position = new Engine.Models.Geometry.Position(1, 1)
        });
        gameState.World.Monsters[0].Monster.Phase = initialPhase;
        if (moved)
        {
            gameState.World.Monsters[0].LastMovementPath = new List<Engine.Models.Geometry.Position>()
            {
                new Engine.Models.Geometry.Position(2, 1),
                new Engine.Models.Geometry.Position(1, 1)
            };
        }

        MonsterSpecialFunctions.PostMonstersAttackFunction(gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
    }

    private GameState SetupNightmareLevel()
    {
        var gameState = new GameState
        {
            Hero = new Engine.Models.Entities.Hero()
            {
                Health = 6,
                Stats = new Dictionary<SkillType, int>()
                {
                    { SkillType.Movement, 2 },
                    { SkillType.Attack, 5 },
                    { SkillType.Defence, 5 },
                    { SkillType.AttackRange, 3 }
                }
            }
        };
        gameState.World.InitializeLevel(GameConstants.NightmareLevelNumber);
        while (gameState.World.Monsters.Count > 1)
        {
            gameState.World.Monsters.RemoveAt(1);
        }

        return gameState;
    }

    [TestCase(1, 2)]
    [TestCase(3, 4)]
    [TestCase(5, 6)]
    public void NightmareTurnStart_WaveDefeated(int phase, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.Monsters[0].Monster.Phase = phase;
        
        MonsterSpecialFunctions.TurnStartMonsterFunctions(gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Stats[SkillType.Defence], Is.EqualTo(1));
        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(GameConstants.NightmareNumberOfRandomWalls));
    }

    [TestCase(-2, 3)]
    [TestCase(-4, 5)]
    public void NightmareTurnStart_SpawnNextWave(int phase, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.InitializeWallBorder();
        gameState.World.Monsters[0].Monster.Phase = phase;

        MonsterSpecialFunctions.TurnStartMonsterFunctions(gameState);

        Assert.That(gameState.World.Monsters.Count, Is.EqualTo(4));
        Assert.That(gameState.World.Monsters[1].Monster.Type, Is.EqualTo(phase == -2 ? MonsterType.Colossus : MonsterType.Reaper));
        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(GameConstants.NightmareNumberOfRandomWalls));
    }

    [TestCase(-6, 7)]
    public void NightmareTurnStart_TransformToBoss(int phase, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.InitializeWallBorder();
        gameState.World.Monsters[0].Monster.Phase = phase;

        MonsterSpecialFunctions.TurnStartMonsterFunctions(gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Health, Is.EqualTo(6));
        Assert.That(gameState.World.Monsters[0].Monster.MaxHealth, Is.EqualTo(6));
        Assert.That(gameState.World.Monsters[0].Monster.IsBossType, Is.True);
        Assert.That(gameState.World.Monsters[0].Monster.BossDiceType, Is.EqualTo(DiceType.D4));
        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Monsters[0].Monster.Stats.Sum(x => x.Value), Is.LessThan(40));
        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(GameConstants.NightmareBossNumberOfRandomWalls));
    }

    [TestCase(-7, 5, false, 7)]
    [TestCase(-7, 4, true, 7)]
    [TestCase(-7, 3, true, 7)]
    [TestCase(-7, 2, true, 7)]
    [TestCase(-7, 1, true, 7)]
    public void NightmareTurnStart_RecreateWalls(int phase, int health, bool expectedSpawnHope, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.InitializeWallBorder();
        gameState.World.Monsters[0].Monster.Phase = phase;
        gameState.World.Monsters[0].Monster.Health = health;
        gameState.World.Monsters[0].Monster.MaxHealth = 6;

        MonsterSpecialFunctions.TurnStartMonsterFunctions(gameState);

        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(GameConstants.NightmareBossNumberOfRandomWalls));
        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Monsters.Count, Is.EqualTo(expectedSpawnHope ? 2 : 1));
        if (expectedSpawnHope)
        {
            Assert.That(gameState.World.Monsters[1].Monster.Type, Is.EqualTo(MonsterType.Hope));
        }
    }

    [TestCase(2, -2)]
    [TestCase(4, -4)]
    [TestCase(6, -6)]
    public void NightmarePostDamage_WavePhase(int phase, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.Monsters[0].Monster.Phase = phase;

        MonsterSpecialFunctions.PostDamageFunction(gameState.World.Monsters[0].Monster, gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(0));
        Assert.That(gameState.World.Monsters[0].Monster.Stats[SkillType.Defence], Is.EqualTo(99));
    }

    [TestCase(7, -7)]
    public void NightmarePostDamage_BossPhase(int phase, int expectedPhase)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.Monsters[0].Monster.Phase = phase;

        MonsterSpecialFunctions.PostDamageFunction(gameState.World.Monsters[0].Monster, gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(expectedPhase));
        Assert.That(gameState.World.Walls.Count - gameState.World.Borders.Count, Is.EqualTo(0));
    }

    [TestCase(6, 6)]
    [TestCase(5, 6)]
    [TestCase(1, 2)]
    public void HopePostDamage_HeroHeal(int health, int expectedHealth)
    {
        var gameState = SetupNightmareLevel();
        gameState.World.Monsters.Add(new MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Hope),
            Position = new Engine.Models.Geometry.Position(2,1)
        });

        gameState.Hero.Health = health;

        MonsterSpecialFunctions.PostDamageFunction(gameState.World.Monsters[1].Monster, gameState);

        Assert.That(gameState.Hero.Health, Is.EqualTo(expectedHealth));
    }
}