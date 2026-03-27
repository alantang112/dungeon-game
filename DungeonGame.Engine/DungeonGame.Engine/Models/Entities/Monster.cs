using System.Collections.Generic;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public class Monster
    {
        public MonsterType Type { get; set; }

        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public Dictionary<SkillType, int> Stats { get; set; } = new Dictionary<SkillType, int>();
    }
}
