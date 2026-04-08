using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameContent
{
    public static class MonsterSpecialFunctions
    {
        private static readonly SkillType[] ColossusLevelUpSkills = { SkillType.Attack, SkillType.Defence, SkillType.AttackRange, SkillType.Movement, SkillType.Attack };     
        public static void PostDamageFunction(Monster monster, GameState gameState)
        {
            switch (monster.Type)
            {
                case MonsterType.Colossus:
                    if (monster.Health >= 1)
                    {
                        var missingHealth = monster.MaxHealth - monster.Health;
                        var nextSkillToLevel = ColossusLevelUpSkills[missingHealth - 1];
                        monster.SetStat(nextSkillToLevel, monster.GetStat(nextSkillToLevel) + 1);
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
                case MonsterType.Elfling:
                    var oathbound = gameState.World.Monsters.FirstOrDefault(x => x.Monster.Type == MonsterType.Oathbound); 
                    if (oathbound != null)
                    {
                        oathbound.Monster.Health--;
                        if (oathbound.Monster.Health <= 0)
                        {
                            gameState.World.Monsters.Remove(oathbound);
                        }
                        else
                        {
                            monster.Health++; // keep elfling alive as long as oathbound is
                            monster.SetStat(SkillType.Defence, monster.GetStat(SkillType.Defence) + GameConstants.ElflingBonusDefence);
                            monster.SetStat(SkillType.Movement, monster.GetStat(SkillType.Movement) + GameConstants.ElflingBonusMovement);
                        }
                    }
                    break;
                case MonsterType.Nightmare:
                    TransformNightmarePostDamage(monster, gameState);
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
                else if (monsterPosition.Monster.Type == MonsterType.Oathbound)
                {
                    monsterPosition.Monster.Phase *= -1;
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
                else if (monsterPosition.Monster.Type == MonsterType.Oathbound)
                {
                    TransformOathbound(monsterPosition);
                    continue;
                }
                else if (monsterPosition.Monster.Type == MonsterType.Elfling)
                {
                    monsterPosition.Monster.SetStat(SkillType.Defence, GameConstants.ElflingBaseDefence);
                    monsterPosition.Monster.SetStat(SkillType.Movement, GameConstants.ElflingBaseMovement);
                    continue;
                }
                else if (monsterPosition.Monster.Type == MonsterType.Nightmare)
                {
                    TransformNightmareTurnStart(monsterPosition, gameState);
                    continue;
                }
            }
        }

        private static void RecalculateDirewolf(MonsterPosition direwolf, GameState gameState)
        {
            if (direwolf.Monster.Type != MonsterType.Direwolf)
                return;

            var neighbouringMonsterCount = gameState.World.Monsters
                .Where(mp => mp.Monster.Id != direwolf.Monster.Id)
                .Where(mp => mp.Monster.Health > 0)
                .Count(mp => GeometryUtility.CalculateDistanceBetween(direwolf.Position, mp.Position) <= (GameConstants.MovementPointsOrthogonal * 2));

            direwolf.Monster.Phase = neighbouringMonsterCount <= 0 ? 1 : (neighbouringMonsterCount == 1 ? 2 : 3);
            direwolf.Monster.SetStat(SkillType.Attack, GameConstants.DirewolfBaseAttack + (neighbouringMonsterCount * GameConstants.DirewolfBonusAttack));
        }

        private static void RecalculateReaperPhase(MonsterPosition reaper)
        {
            var moved = reaper.LastMovementPath.Any();

            if (reaper.Monster.Phase == 2 && moved)
            {
                reaper.Monster.Phase = -2;
            }
            else if (reaper.Monster.Phase == 1 && !moved)
            {
                reaper.Monster.Phase = -1;
            }
        }

        private static void TransformReaper(MonsterPosition reaper)
        {
            if (reaper.Monster.Phase == -2)
            {
                reaper.Monster.Phase = 1;
                reaper.Monster.SetStat(SkillType.Attack, GameConstants.ReaperBaseAttack);
                reaper.Monster.SetStat(SkillType.Movement, GameConstants.ReaperBaseMovement);
            }
            else if (reaper.Monster.Phase == -1)
            {
                reaper.Monster.Phase = 2;
                reaper.Monster.SetStat(SkillType.Attack, GameConstants.ReaperEmpoweredAttack);
                reaper.Monster.SetStat(SkillType.Movement, GameConstants.ReaperEmpoweredMovement);
            }
        }

        private static void TransformOathbound(MonsterPosition oathbound)
        {
            if (oathbound.Monster.Phase == -1)
            {
                oathbound.Monster.Phase = 2;
                oathbound.Monster.SetStat(SkillType.Movement, GameConstants.OathboundPhase2Movement);
                oathbound.Monster.SetStat(SkillType.Attack, GameConstants.OathboundPhase2Attack);
            }
            else if (oathbound.Monster.Phase == -2)
            {
                
                oathbound.Monster.Phase = 1;
                oathbound.Monster.SetStat(SkillType.Movement, GameConstants.OathboundPhase1Movement);
                oathbound.Monster.SetStat(SkillType.Attack, GameConstants.OathboundPhase1Attack);
            }
        }

        private static void TransformNightmareTurnStart(MonsterPosition monsterPosition, GameState gameState)
        {
            switch (monsterPosition.Monster.Phase)
            {
                // Wave monsters defeated -> nightmare damaged
                case 1:
                case 3:
                case 5:
                    if (!gameState.World.Monsters.Any(mp => mp.Monster.Id != monsterPosition.Monster.Id))
                    {
                        monsterPosition.Monster.SetStat(SkillType.Defence, 1);
                        monsterPosition.Monster.Phase += 1;
                    }
                    break;
                // Spawn next wave
                case -2:
                case -4:
                    MonsterType[] waveMonsters = monsterPosition.Monster.Phase == -2 
                        ? new MonsterType[] { MonsterType.Colossus, MonsterType.Fiendling, MonsterType.Direwolf }
                        : new MonsterType[] { MonsterType.Reaper, MonsterType.Oathbound, MonsterType.Elfling };

                    var spawnPositions = gameState.World.FindSpawnPositions(waveMonsters.Length);
                    foreach(var (monsterType, index) in waveMonsters.Select((v, i) => (v, i)))
                    {
                        gameState.World.Monsters.Add(new MonsterPosition()
                        {
                            Monster = MonsterSpawner.Spawn(monsterType),
                            Position = spawnPositions[index]
                        });
                    }

                    gameState.World.InitializeRandomWalls(GameConstants.NightmareNumberOfRandomWalls, true);
                    monsterPosition.Monster.Phase = (monsterPosition.Monster.Phase * -1) + 1;
                    break;
                // Final wave defeated, transform to Boss mode
                case -6:
                    monsterPosition.Monster.Health = 6;
                    monsterPosition.Monster.MaxHealth = monsterPosition.Monster.Health;
                    monsterPosition.Monster.SetStat(SkillType.Movement, 4);
                    monsterPosition.Monster.SetStat(SkillType.Attack, 8);
                    monsterPosition.Monster.SetStat(SkillType.Defence, 8);
                    monsterPosition.Monster.SetStat(SkillType.AttackRange, 6);
                    monsterPosition.Monster.IsBossType = true;
                    monsterPosition.Monster.BossDiceType = DiceType.D4;
                    
                    gameState.World.InitializeRandomWalls(GameConstants.NightmareBossNumberOfRandomWalls, false);
                    monsterPosition.Monster.Phase = 7;
                    break;
                // Boss damaged, recreate level walls
                case -7:
                    gameState.World.InitializeRandomWalls(GameConstants.NightmareBossNumberOfRandomWalls, false);
                    monsterPosition.Monster.Phase = 7;
                    break;
            }
        }

        private static void TransformNightmarePostDamage(Monster nightmare, GameState gameState)
        {
            switch (nightmare.Phase)
            {
                case 2:
                case 4:
                case 6:
                    nightmare.Health = 1;
                    nightmare.SetStat(SkillType.Defence, 99);
                    gameState.World.InitializeWallBorder();
                    nightmare.Phase *= -1;
                    break;
                case 7:
                    gameState.World.InitializeWallBorder();
                    nightmare.Phase *= -1;
                    break;
            }
        }
    }
}
