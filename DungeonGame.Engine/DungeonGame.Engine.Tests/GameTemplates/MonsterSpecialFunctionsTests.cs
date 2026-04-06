using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Tests.GameTemplates;

public class MonsterSpecialFunctionsTests
{
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 5, SkillType.Defence)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 4, SkillType.AttackRange)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 3, SkillType.Movement)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 2, SkillType.Attack)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 1, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 5, SkillType.Defence)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 4, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 3, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 2, SkillType.AttackRange)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 1, SkillType.Movement)]
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

    [TestCase(true)]
    [TestCase(false)]
    public void GivenReaper_AndMoved_WhenPostMonstersAttack_RecalculateState(bool moved)
    {
        var gameState = new GameState();

        gameState.World.InitializeLevel(-1);

        gameState.World.Monsters.Add(new Engine.Models.Entities.MonsterPosition()
        {
            Monster = MonsterSpawner.Spawn(MonsterType.Reaper),
            Position = new Engine.Models.Geometry.Position(1, 1)
        });
        if (moved)
        {
            gameState.World.Monsters[0].LastMovementPath = new List<Engine.Models.Geometry.Position>()
            {
                new Engine.Models.Geometry.Position(2, 1),
                new Engine.Models.Geometry.Position(1, 1)
            };
        }

        MonsterSpecialFunctions.PostMonstersAttackFunction(gameState);

        Assert.That(gameState.World.Monsters[0].Monster.Phase, Is.EqualTo(moved ? -1 : -2));
    }
}