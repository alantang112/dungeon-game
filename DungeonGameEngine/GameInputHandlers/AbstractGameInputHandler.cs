using System;
using System.Linq;
using DungeonGameEngine.Models;

namespace DungeonGameEngine.GameInputHandlers
{
    internal abstract class AbstactGameInputHandler : IGameInputHandler
    {
        private IGameInputHandler? _nextHandler;

        protected abstract GamePhase[] HandledGamePhases { get; }
        protected abstract InputEventType[] HandledInputEventTypes { get; }

        public IGameInputHandler SetNext(IGameInputHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public GameState Handle(GameState gameState, InputEvent inputEvent)
        {
            if (HandledGamePhases.Contains(gameState.GamePhase) && HandledInputEventTypes.Contains(inputEvent.EventType))
            {
                return TransformGameState(gameState, inputEvent);
            }

            if (_nextHandler != null)
            {
                return _nextHandler.Handle(gameState, inputEvent);
            }
            else
            {
                throw new NotSupportedException("Input event not supported for current game state");
            }
        }

        public abstract GameState TransformGameState(GameState gameState, InputEvent inputEvent);
    }
}
