using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    public class NextLevelEventParameters : IInputEventParameters
    {
        public SkillType? SkillType { get; set; }
        public bool ReplenishHealth { get; set; }
    }
}
