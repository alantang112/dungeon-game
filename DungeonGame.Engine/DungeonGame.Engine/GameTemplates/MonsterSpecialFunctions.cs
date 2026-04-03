using System;
using System.Linq;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.GameTemplates
{
    public static class MonsterSpecialFunctions
    {
        public static void PostDamageFunction(Monster monster)
        {
            switch (monster.Type)
            {
                case MonsterType.Colossus:
                    // level a random stat
                    var randomStat = monster.Stats.OrderBy(_ => Guid.NewGuid()).First();
                    monster.Stats[randomStat.Key]++;
                    break;
            }
        }
    }
}