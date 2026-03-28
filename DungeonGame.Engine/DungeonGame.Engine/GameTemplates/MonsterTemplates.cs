using System;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameTemplates
{
    public static class MonsterSpawner
    {
        public static Monster Spawn(MonsterType type)
        {
            var monster = new Monster
            {
                Type = type,
                Name = CharacterNameUtility.GetRandomName()
            };

            switch (type)
            {
                case MonsterType.Spider:
                    monster.Health = 2;
                    monster.MaxHealth = monster.Health;
                    monster.Stats.Add(SkillType.Movement, 5);
                    monster.Stats.Add(SkillType.Attack, 4);
                    monster.Stats.Add(SkillType.Defence, 4);
                    monster.Stats.Add(SkillType.AttackRange, 3);
                    break;
                case MonsterType.Skeleton:
                    monster.Health = 3;
                    monster.MaxHealth = monster.Health;
                    monster.Stats.Add(SkillType.Movement, 4);
                    monster.Stats.Add(SkillType.Attack, 3); // 5 -> 3
                    monster.Stats.Add(SkillType.Defence, 4);
                    monster.Stats.Add(SkillType.AttackRange, 4);
                    break;
                case MonsterType.Cyclops:
                    monster.Health = 5;
                    monster.MaxHealth = monster.Health;
                    monster.Stats.Add(SkillType.Movement, 3);
                    monster.Stats.Add(SkillType.Attack, 7);
                    monster.Stats.Add(SkillType.Defence, 7);
                    monster.Stats.Add(SkillType.AttackRange, 2);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return monster;
        }
    }
}
