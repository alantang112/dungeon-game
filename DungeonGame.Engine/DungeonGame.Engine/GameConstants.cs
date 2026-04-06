namespace DungeonGame.Engine
{
    public static class GameConstants
    {
        public static int DiceMin = 1;
        public static int DiceMax = 6;
        public static int NumberOfEnergyDice = 3;

        public static int LevelSize = 5;

        public static int MovementPointsDiagonal = 3;
        public static int MovementPointsOrthogonal = 2;

        public static double GeometryCalculationEpsilon = 0.00001;
        public static int GeometryCalculationDecimalPlaces = 5;

        public static int GameMessageLogLimit = 20;

        public static int HeroMaxHealth = 6;

        public static int LoopIterationLimit = 100;

        public static int DirewolfBaseAttack = 4;
        public static int DirewolfBonusAttack = 2;
    }
}
