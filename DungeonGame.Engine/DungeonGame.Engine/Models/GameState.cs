using System.Collections.Generic;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;

namespace DungeonGame.Engine.Models
{
    public class GameState
    {
        public GamePhase GamePhase { get; set; } = GamePhase.Start;
        public EnergyDice EnergyDice { get; set; } = new EnergyDice();
        public Hero Hero { get; set; } = new Hero();
        public World World { get; set; } = new World();
        public World? WorldSnapshot { get; set; }
        public int? LevelNumber { get; set; }
        public string? GameMessage { get; set;}
        public List<InputEvent> ScheduledEvents { get; set;} = new List<InputEvent>();
    }
}
