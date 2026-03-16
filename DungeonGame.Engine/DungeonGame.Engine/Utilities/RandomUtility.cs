using System;

namespace DungeonGame.Engine.Utilities
{
    public class RandomUtility
    {
        public static Random Random = new Random();

        public static int RandomInt(int min, int max) => Random.Next(min, max + 1);
    }
}
