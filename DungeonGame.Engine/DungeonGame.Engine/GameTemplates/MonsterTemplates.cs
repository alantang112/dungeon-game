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
                    monster.Stats.Add(SkillType.Attack, 5);
                    monster.Stats.Add(SkillType.Defence, 4);
                    monster.Stats.Add(SkillType.AttackRange, 4);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return monster;
        }
    }
}
