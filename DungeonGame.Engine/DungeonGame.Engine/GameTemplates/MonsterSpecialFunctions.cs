using System.Linq;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameTemplates
{
    public static class MonsterSpecialFunctions
    {
        private static readonly SkillType[] ColossusLevelUpSkills = { SkillType.Attack, SkillType.Attack, SkillType.Defence, SkillType.Movement, SkillType.AttackRange };     
        public static void PostDamageFunction(Monster monster)
        {
            switch (monster.Type)
            {
                case MonsterType.Colossus:
                    if (monster.Health >= 1)
                    {
                        // deterministically level a random stat
                        var missingHealth = monster.MaxHealth - monster.Health;
                        var enumeratedSkillToLevel = ColossusLevelUpSkills
                                .Select((skillType, index) => (skillType, index))
                                .OrderBy(tup => RandomUtility.GenerateDeterministicGuid(monster.RandomSeed, tup.index))
                                .Skip(missingHealth - 1)
                                .First();
                        monster.SetStat(enumeratedSkillToLevel.skillType, monster.GetStat(enumeratedSkillToLevel.skillType) + 1);
                    }
                    break;
                case MonsterType.Overseer:
                    if (monster.Health <= 0)
                    {
                        if (monster.GetStat(SkillType.Movement, true) != 2)
                        {
                            monster.SetStat(SkillType.Movement, 2);
                            monster.SetStat(SkillType.Attack, 5);
                            monster.SetStat(SkillType.Defence, 5);
                            monster.SetStat(SkillType.AttackRange, 4);
                            monster.Health = 6;
                        }
                    }
                    break;
            }
        }
    }
}
