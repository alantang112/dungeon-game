using System.Collections.Generic;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public class Hero
    {
        public string Name { get; set; }
        public Dictionary<SkillType, int> Stats { get; set; } = new Dictionary<SkillType, int>();
    }
}
