namespace DungeonGameEngine.Models.InputEventModels
{
    public class InputEvent
    {
        public InputEventType EventType { get; set; }
        public IInputEventParameters? EventParameters { get; set;}
    }
}
