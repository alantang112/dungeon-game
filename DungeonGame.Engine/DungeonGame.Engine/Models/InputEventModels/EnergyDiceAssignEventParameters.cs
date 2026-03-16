using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.InputEventModels
{
    public class EnergyDiceAssignEventParameters : IInputEventParameters
    {
        public int DiceIndex { get; set; }
        public SkillType SkillType { get; set; }
    }
}
