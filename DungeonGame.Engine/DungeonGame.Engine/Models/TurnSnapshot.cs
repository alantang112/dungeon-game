using DungeonGame.Engine.Models.Entities;


namespace DungeonGame.Engine.Models
{
    public class TurnSnapshot
    {
        public Hero Hero { get; set; }
        public EnergyDice EnergyDice { get; set; }
        public World World { get; set; }
    }
}
