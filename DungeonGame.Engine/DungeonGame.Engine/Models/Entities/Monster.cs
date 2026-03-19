using System.Collections.Generic;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public class Monster
    {
        public MonsterType Type { get; set; }

        public int Health { get; set; }
        public Dictionary<SkillType, int> Stats { get; set; }
    }
}
