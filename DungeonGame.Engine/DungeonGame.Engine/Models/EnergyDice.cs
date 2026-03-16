using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class EnergyDice
    {
        public int[] Dice { get; set; } = new int[3];
        public void Roll()
        {
            for(var i = 0; i < Dice.Length; i++)
            {
                Dice[i] = RandomUtility.RandomInt(1, 6);
            }
        }
    }
}
