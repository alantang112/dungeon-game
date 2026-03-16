using System;
using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.GameInputHandlers
{
    internal abstract class AbstactGameInputHandler : IGameInputHandler
    {
        private IGameInputHandler? _nextHandler;

        protected abstract GamePhase HandledGamePhase { get; }
        protected abstract InputEventType[] HandledInputEventTypes { get; }

        public IGameInputHandler SetNext(IGameInputHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public GameState Handle(GameState gameState, InputEvent inputEvent)
        {
            if (HandledGamePhase == gameState.GamePhase)
            {
                if (HandledInputEventTypes.Contains(inputEvent.EventType))
                {
                    return TransformGameState(gameState, inputEvent);
                }
                
                throw new NotSupportedException($"Input event {inputEvent.EventType} not supported for current game phase {gameState.GamePhase}");
            }

            if (_nextHandler != null)
            {
                return _nextHandler.Handle(gameState, inputEvent);
            }
            
            throw new NotImplementedException($"Game phase handler {gameState.GamePhase} not implemented yet");
        }

        public abstract GameState TransformGameState(GameState gameState, InputEvent inputEvent);
    }
}
