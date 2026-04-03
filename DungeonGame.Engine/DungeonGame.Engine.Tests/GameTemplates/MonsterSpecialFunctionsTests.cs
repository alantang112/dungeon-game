using DungeonGame.Engine.GameTemplates;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Tests.GameTemplates;

public class MonsterSpecialFunctionsTests
{
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 5, SkillType.Attack)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 4, SkillType.Attack)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 3, SkillType.Defence)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 2, SkillType.Movement)]
    [TestCase("6946ddcd-ff4b-4798-845d-75c2c599e143", 1, SkillType.AttackRange)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 5, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 4, SkillType.Attack)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 3, SkillType.Defence)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 2, SkillType.Movement)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 1, SkillType.AttackRange)]
    [TestCase("c04d22e4-92f6-4293-a0b7-9babefacfe72", 0, null)]
    public void GivenColossusWithRandomSeed_ThenDeterministicallyLevelStat(string guid, int currentHealth, SkillType? expectedSkillTypeLeveled)
    {
        var monster = MonsterSpawner.Spawn(Engine.Models.Enums.MonsterType.Colossus);
        monster.Health = currentHealth;

        monster.RandomSeed = new Guid(guid);

        var initialStats = new Dictionary<SkillType, int>(monster.Stats);

        MonsterSpecialFunctions.PostDamageFunction(monster);

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
}