using DungeonGameEngine.Models;
using DungeonGameEngine.Models.InputEventModels;

namespace DungeonGameEngine.GameInputHandlers
{
    internal interface IGameInputHandler
    {
        public IGameInputHandler SetNext(IGameInputHandler handler);

        public GameState Handle(GameState gameState, InputEvent inputEvent);
    }
}

