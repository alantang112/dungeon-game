using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    public class UpgradeHeroEventParameters : IInputEventParameters
    {
        public SkillType? SkillType { get; set; }
        public bool ReplenishHealth { get; set; }
    }
}
