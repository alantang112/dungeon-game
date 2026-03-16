using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models
{
    public class GameState
    {
        public GamePhase GamePhase { get; set; } = GamePhase.Start;
        public EnergyDice EnergyDice { get; set; } = new EnergyDice();
    }
}
