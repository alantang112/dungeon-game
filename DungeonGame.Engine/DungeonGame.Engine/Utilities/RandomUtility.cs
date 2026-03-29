using System;

namespace DungeonGame.Engine.Utilities
{
    public class RandomUtility
    {
        private static Random _random = new Random();

        public static double Random() => _random.NextDouble();
        public static int RandomInt(int min, int max) => _random.Next(min, max + 1);

        public static bool RandomBool() => RandomInt(1, 2) == 1;
    }
}
