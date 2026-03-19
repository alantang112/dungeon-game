using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models
{
    public class GameState
    {
        public GamePhase GamePhase { get; set; } = GamePhase.Start;
        public EnergyDice EnergyDice { get; set; } = new EnergyDice();
        public Hero Hero { get; set; } = new Hero();
        public World World { get; set; } = new World();
        public int? LevelNumber { get; set; }
        public string? GameMessage { get; set;}
    }
}
