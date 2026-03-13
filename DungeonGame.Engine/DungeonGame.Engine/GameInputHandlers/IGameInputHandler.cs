using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers
{
    internal interface IGameInputHandler
    {
        public IGameInputHandler SetNext(IGameInputHandler handler);

        public GameState Handle(GameState gameState, InputEvent inputEvent);
    }
}

