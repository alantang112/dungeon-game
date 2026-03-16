using System;

namespace DungeonGame.Engine
{
    public static class GameConstants
    {
        public static int DiceMin = 1;
        public static int DiceMax = 3;
        public static int NumberOfEnergyDice = 3;

        public static int LevelSize = 5;

        // Game Messages
        public static string SkillAlreadyAssignedEnergyDice = "This skill type is already assigned an energy dice";
        public static string InvalidSkillForEnergyDiceAssignment = "This skill type cannot be assigned an energy dice";
        public static string AssignAllEnergyDiceBeforeProceeding = "Please assign all energy dice before proceeding";
    }
}
