namespace DungeonGame.Engine.Models.InputEventModels
{
    public class HeroActionMoveEventParameters : IInputEventParameters
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
