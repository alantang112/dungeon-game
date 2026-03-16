using System;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Entities.Monsters;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Utilities
{
    public static class MonsterSpawner
    {
        public static Monster Spawn(MonsterType type)
        {
            switch (type)
            {
                case MonsterType.Spider:
                    return new Spider();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
