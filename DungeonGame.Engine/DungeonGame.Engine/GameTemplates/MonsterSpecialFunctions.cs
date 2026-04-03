using System.Linq;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;

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
                    if (monster.Health > 1)
                    {
                        // level a deterministically random stat
                        var missingHealth = monster.MaxHealth - monster.Health;
                        var skillToLevel = ColossusLevelUpSkills.OrderBy(x => monster.RandomSeed).Skip(missingHealth - 1).First();
                        monster.SetStat(skillToLevel, monster.GetStat(skillToLevel) + 1);
                    }
                    break;
                case MonsterType.Overseer:
                    if (monster.Health <= 0)
                    {
                        if (monster.GetStat(SkillType.Movement) != 2)
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
