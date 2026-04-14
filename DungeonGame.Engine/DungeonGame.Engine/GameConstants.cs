namespace DungeonGame.Engine
{
    public static class GameConstants
    {
        public const int DiceMin = 1;
        public const int DiceMax = 6;
        public const int NumberOfEnergyDice = 3;

        public const int LevelSize = 5;

        public const int MovementPointsDiagonal = 3;
        public const int MovementPointsOrthogonal = 2;

        public const double GeometryCalculationEpsilon = 0.00001;
        public const int GeometryCalculationDecimalPlaces = 5;

        public const int GameMessageLogLimit = 20;

        public const int HeroMaxHealth = 6;

        public const int LoopIterationLimit = 100;

        public const int DirewolfBaseAttack = 4;
        public const int DirewolfBonusAttack = 2;
        public const int ReaperBaseAttack = 4;
        public const int ReaperEmpoweredAttack = 99;
        public const int ReaperBaseMovement = 6;
        public const int ReaperEmpoweredMovement = 2;
        public const int OathboundPhase1Movement = 5;
        public const int OathboundPhase1Attack = 9;
        public const int OathboundPhase2Movement = 3;
        public const int OathboundPhase2Attack = 12;
        public const int ElflingBaseDefence = 3;
        public const int ElflingBonusDefence = 7;
        public const int ElflingBaseMovement = 5;
        public const int ElflingBonusMovement = 5;
        public const int NightmareLevelNumber = 16;
        public const int NightmareNumberOfRandomWallsWave1 = 4;

        public const int NightmareNumberOfRandomWallsWave2 = 4;
        public const int NightmareNumberOfRandomWallsWave3 = 2;
        public const int NightmareBossNumberOfRandomWalls = 6;
    }
}
