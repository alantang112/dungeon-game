using System.Collections.Generic;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class GameState
    {
        public GamePhase GamePhase { get; set; } = GamePhase.Start;
        public EnergyDice EnergyDice { get; set; } = new EnergyDice();
        public Hero Hero { get; set; } = new Hero();
        public World World { get; set; } = new World();
        public int? LevelNumber { get; set; }
        public string? GameMessage { get; set; }
        public List<InputEvent> ScheduledEvents { get; set;} = new List<InputEvent>();
        public List<string> GameMessageLog { get; set; } = new List<string>();
        public ViewData ViewData { get; set; } = new ViewData();

        // snapshots
        public Hero? HeroSnapshot { get; set; }
        public EnergyDice? EnergyDiceSnapshot { get; set; }
        public World? WorldSnapshot { get; set; }

        public void AddGameMessage(string message)
        {
            GameMessage = message;
            GameMessageLog.Add(message);

            if (GameMessageLog.Count > GameConstants.GameMessageLogLimit)
                GameMessageLog.RemoveAt(0);
        }

        public void ClearGameMessage()
        {
            GameMessage = null;
        }

        public void LoadTurnSnapshot()
        {
            Hero = HeroSnapshot.DeepClone();
            World = WorldSnapshot.DeepClone();
            EnergyDice = EnergyDiceSnapshot.DeepClone();
        }

        public void SaveTurnSnapshot()
        {
            HeroSnapshot = Hero.DeepClone();
            EnergyDiceSnapshot = EnergyDice.DeepClone();
            WorldSnapshot = World.DeepClone();
        }
    }
}
