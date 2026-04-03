using System;
using System.Security.Cryptography;

namespace DungeonGame.Engine.Utilities
{
    public class RandomUtility
    {
        private static Random _random = new Random();

        public static double Random() => _random.NextDouble();
        public static int RandomInt(int min, int max) => _random.Next(min, max + 1);

        public static bool RandomBool() => RandomInt(1, 2) == 1;

        public static Guid GenerateDeterministicGuid(Guid baseGuid, int seed)
        {
            // 1. Combine the inputs into a single byte array
            byte[] guidBytes = baseGuid.ToByteArray();
            byte[] seedBytes = BitConverter.GetBytes(seed);
            
            byte[] combinedBytes = new byte[guidBytes.Length + seedBytes.Length];
            Buffer.BlockCopy(guidBytes, 0, combinedBytes, 0, guidBytes.Length);
            Buffer.BlockCopy(seedBytes, 0, combinedBytes, guidBytes.Length, seedBytes.Length);

            // 2. Hash the combined data
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(combinedBytes);

                // 3. Take the first 16 bytes of the hash to create the new GUID
                byte[] newGuidBytes = new byte[16];
                Array.Copy(hash, 0, newGuidBytes, 0, 16);

                return new Guid(newGuidBytes);
            }
        }
    }
}
