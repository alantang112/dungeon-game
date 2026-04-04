using System;
using System.Collections.Generic;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public class Monster
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public MonsterType Type { get; set; }
        public bool IsBossType { get; set; } = false;
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public Dictionary<SkillType, int> Stats { get; set; } = new Dictionary<SkillType, int>();
        public Guid RandomSeed { get; set; }

        public void SetStat(SkillType skillType, int value)
        {
            if (Stats.ContainsKey(skillType))
                Stats[skillType] = value;
            else
                Stats.Add(skillType, value);
        }

        public int GetStat(SkillType skillType, bool baseValueOnly = false)
        {
            var baseValue = Stats[skillType];

            if (!IsBossType || baseValueOnly)
                return baseValue;

            var bossDice = BossDice.ContainsKey(skillType) ? BossDice[skillType] : 0;

            return baseValue + bossDice;
        }

        public Dictionary<SkillType, int> BossDice { get; set; } = new Dictionary<SkillType, int>();
        public DiceType? BossDiceType { get; set; }
    }
}
