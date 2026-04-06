using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameContent
{
    public static class MonsterSpecialFunctions
    {
        private static readonly SkillType[] ColossusLevelUpSkills = { SkillType.Attack, SkillType.Attack, SkillType.Defence, SkillType.Movement, SkillType.AttackRange };     
        public static void PostDamageFunction(Monster monster, GameState gameState)
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
                        if (monster.Phase == 1)
                        {
                            monster.SetStat(SkillType.Movement, 3);
                            monster.SetStat(SkillType.Attack, 5);
                            monster.SetStat(SkillType.Defence, 6);
                            monster.SetStat(SkillType.AttackRange, 6);
                            monster.Health = 6;
                            monster.Phase = 2;
                        }
                    }
                    break;
                case MonsterType.Direwolf:
                    if (monster.Health <= 0)
                    {
                        foreach(var otherMonster in gameState.World.Monsters.Where(mp => mp.Monster.Id != monster.Id))
                        {
                            if (otherMonster.Monster.Type == MonsterType.Direwolf)
                                RecalculateDirewolf(otherMonster, gameState);
                        }
                    }
                    break;
            }
        }

        public static void PostMonstersMoveFunction(GameState gameState)
        {
            foreach(var monsterPosition in gameState.World.Monsters)
            {
                if (monsterPosition.Monster.Type == MonsterType.Direwolf)
                {
                    RecalculateDirewolf(monsterPosition, gameState);
                    continue;
                }
            }
        }

        public static void PostMonstersAttackFunction(GameState gameState)
        {
            foreach(var monsterPosition in gameState.World.Monsters)
            {
                if (monsterPosition.Monster.Type == MonsterType.Reaper)
                {
                    RecalculateReaperPhase(monsterPosition);
                    continue;
                }
            }
        }

        public static void TurnStartMonsterFunctions(GameState gameState)
        {
            foreach(var monsterPosition in gameState.World.Monsters)
            {
                if (monsterPosition.Monster.Type == MonsterType.Reaper)
                {
                    TransformReaper(monsterPosition);
                    continue;
                }
            }
        }

        private static void RecalculateDirewolf(MonsterPosition direwolf, GameState gameState)
        {
            if (direwolf.Monster.Type != MonsterType.Direwolf)
                return;

            var neighbouringDirewolfCount = gameState.World.Monsters
                .Where(mp => mp.Monster.Id != direwolf.Monster.Id)
                .Where(mp => mp.Monster.Health > 0)
                .Count(mp => GeometryUtility.CalculateDistanceBetween(direwolf.Position, mp.Position) <= (GameConstants.MovementPointsOrthogonal * 2));

            direwolf.Monster.Phase = neighbouringDirewolfCount <= 0 ? 1 : (neighbouringDirewolfCount == 1 ? 2 : 3);
            direwolf.Monster.SetStat(SkillType.Attack, GameConstants.DirewolfBaseAttack + (neighbouringDirewolfCount * GameConstants.DirewolfBonusAttack));
        }

        private static void RecalculateReaperPhase(MonsterPosition reaper)
        {
            var moved = reaper.LastMovementPath.Any();

            if (moved)
            {
                reaper.Monster.Phase = -1;
            }
            else if (reaper.Monster.Phase == 1)
            {
                reaper.Monster.Phase = -2;
            }
        }

        private static void TransformReaper(MonsterPosition reaper)
        {
            if (reaper.Monster.Phase == -1)
            {
                reaper.Monster.Phase = 1;
                reaper.Monster.SetStat(SkillType.Attack, GameConstants.ReaperBaseAttack);
            }
            else if (reaper.Monster.Phase == -2)
            {
                reaper.Monster.Phase = 2;
                reaper.Monster.SetStat(SkillType.Attack, GameConstants.ReaperEmpoweredAttack);
            }
        }
    }
}
