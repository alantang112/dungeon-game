using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class EnergyDice
    {
        public int[] Dice { get; set; } = new int[GameConstants.NumberOfEnergyDice];
        public SkillType?[] AssignedSkills { get; set; } = new SkillType?[GameConstants.NumberOfEnergyDice];

        public void Roll()
        {
            for(var i = 0; i < Dice.Length; i++)
            {
                Dice[i] = RandomUtility.RandomInt(GameConstants.DiceMin, GameConstants.DiceMax);
            }
        }

        public void AssignDice(int index, SkillType skillType)
        {
            AssignedSkills[index] = skillType;
        }

        public void ResetAssignment()
        {
            AssignedSkills = new SkillType?[GameConstants.NumberOfEnergyDice];
        }
    }
}
