using System;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.GameTemplates
{
    public static class MonsterSpawner
    {
        public static Monster Spawn(MonsterType type)
        {
            var monster = new Monster() { Type = type };

            switch (type)
            {
                case MonsterType.Spider:
                    break;
                default:
                    throw new NotImplementedException();
            }

            return monster;
        }
    }
}
