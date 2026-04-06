using System;
using System.Collections.Generic;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public class Hero
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public bool isMaleName { get; set; }
        public int Health { get; set; }
        public Dictionary<SkillType, int> Stats { get; set; } = new Dictionary<SkillType, int>();
    }
}
