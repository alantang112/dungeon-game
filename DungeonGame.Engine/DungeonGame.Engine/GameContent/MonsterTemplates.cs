using System;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameContent
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
                    monster.SetStat(SkillType.Defence, 6);
                    monster.SetStat(SkillType.AttackRange, 2);
                    break;
                case MonsterType.Overseer:
                    monster.IsBossType = true;
                    monster.Phase = 1;
                    monster.BossDiceType = DiceType.D4;
                    monster.Health = 6;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 1);
                    monster.SetStat(SkillType.Attack, 3);
                    monster.SetStat(SkillType.Defence, 3);
                    monster.SetStat(SkillType.AttackRange, 4);
                    break;
                case MonsterType.Direwolf:
                    monster.Health = 3;
                    monster.Phase = 1;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, 4);
                    monster.SetStat(SkillType.Attack, GameConstants.DirewolfBaseAttack);
                    monster.SetStat(SkillType.Defence, 6);
                    monster.SetStat(SkillType.AttackRange, 3);
                    break;
                case MonsterType.Reaper:
                    monster.Health = 4;
                    monster.Phase = 1;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, GameConstants.ReaperBaseMovement);
                    monster.SetStat(SkillType.Attack, GameConstants.ReaperBaseAttack);
                    monster.SetStat(SkillType.Defence, 6);
                    monster.SetStat(SkillType.AttackRange, 3);
                    break;
                case MonsterType.Oathbound:
                    monster.Health = 6;
                    monster.Phase = 1;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, GameConstants.OathboundPhase1Movement);
                    monster.SetStat(SkillType.Attack, GameConstants.OathboundPhase1Attack);
                    monster.SetStat(SkillType.Defence, 99);
                    monster.SetStat(SkillType.AttackRange, 2);
                    break;
                case MonsterType.Elfling:
                    monster.Health = 1;
                    monster.MaxHealth = monster.Health;
                    monster.SetStat(SkillType.Movement, GameConstants.ElflingBaseMovement);
                    monster.SetStat(SkillType.Attack, 3);
                    monster.SetStat(SkillType.Defence, GameConstants.ElflingBaseDefence);
                    monster.SetStat(SkillType.AttackRange, 2);
                    monster.Traits = new MonsterTrait[] { MonsterTrait.FleeFromHero };
                    break;
                case MonsterType.Nightmare:
                    monster.Health = 4;
                    monster.MaxHealth = monster.Health;
                    monster.Phase = 1;
                    monster.SetStat(SkillType.Movement, 2);
                    monster.SetStat(SkillType.Attack, 99);
                    monster.SetStat(SkillType.Defence, 99);
                    monster.SetStat(SkillType.AttackRange, 2);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return monster;
        }
    }
}
