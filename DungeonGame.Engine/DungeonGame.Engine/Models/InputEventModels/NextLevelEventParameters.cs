using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    public class NextLevelEventParameters : IInputEventParameters
    {
        public SkillType? LevelUpSkill { get; set; }
        public bool ReplenishHealth { get; set; }
    }
}
