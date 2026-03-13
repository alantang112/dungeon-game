using System;
using DungeonGameEngine.Models;

namespace DungeonGameEngine
{
    public class GameEngine : IGameEngine
    {
        public GameState CurrentState { get; private set; }

        public event Action OnStateChanged = delegate { };

        public GameEngine()
        {
            CurrentState = new GameState();
        }

        public void ProcessInput(InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.NewGame)
            {
                CurrentState.GamePhase = GamePhase.GameEnergyDicePreRoll;
                OnStateChanged?.Invoke();
                return;
            }

            throw new ArgumentException("Invalid input event type for state");
            // TODO: implement handler
        }
    }
}
