using System;
using System.Linq;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameTemplates
{
    public static class MonsterSpawner
    {
        public static Monster Spawn(MonsterType type)
        {
            var (name, _) = CharacterNameUtility.GetRandomName();

            var monster = new Monster
            {
                Type = type,
                Name = name,
                RandomSeed = Guid.NewGuid()
            };

            switch (type)
            {
                case MonsterType.Spider:
                    monster.Health = 2;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 5);
                    monster.SetStat(SkillType.Attack, 4);
                    monster.SetStat(SkillType.Defence, 4);
                    monster.SetStat(SkillType.AttackRange, 3);
                    break;
                case MonsterType.Skeleton:
                    monster.Health = 3;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 2);
                    monster.SetStat(SkillType.Attack, 3);
                    monster.SetStat(SkillType.Defence, 3);
                    monster.SetStat(SkillType.AttackRange, 5);
                    break;
                case MonsterType.Minotaur:
                    monster.Health = 5;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 3);
                    monster.SetStat(SkillType.Attack, 7);
                    monster.SetStat(SkillType.Defence, 7);
                    monster.SetStat(SkillType.AttackRange, 2);
                    break;
                case MonsterType.Fiendling:
                    monster.Health = 5;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 4);
                    monster.SetStat(SkillType.Attack, 5);
                    monster.SetStat(SkillType.Defence, 5);
                    monster.SetStat(SkillType.AttackRange, 4);
                    break;
                case MonsterType.Colossus:
                    monster.Health = 6;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 3);
                    monster.SetStat(SkillType.Attack, 5);
                    monster.SetStat(SkillType.Defence, 5);
                    monster.SetStat(SkillType.AttackRange, 2);
                    break;
                case MonsterType.Overseer:
                    monster.IsBossType = true;
                    monster.BossDiceType = DiceType.D4;
                    monster.Health = 6;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 1);
                    monster.SetStat(SkillType.Attack, 3);
                    monster.SetStat(SkillType.Defence, 3);
                    monster.SetStat(SkillType.AttackRange, 4);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return monster;
        }
    }
}
