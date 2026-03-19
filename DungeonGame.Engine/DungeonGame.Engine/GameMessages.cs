namespace DungeonGame.Engine
{
    public static class GameMessages
    {
        public static string SkillAlreadyAssignedEnergyDice = "This skill type is already assigned an energy dice";
        public static string InvalidSkillForEnergyDiceAssignment = "This skill type cannot be assigned an energy dice";
        public static string AssignAllEnergyDiceBeforeProceeding = "Please assign all energy dice before proceeding";
        public static string NotEnoughMovementActionPoints = "You do not have enough movement action points";
        public static string CanOnlyMoveAdjacently = "You can only move to adjacent spaces";
        public static string CannotMoveToThatSpace = "You cannot move to that space";
        public static string NoMonsterToAttackAtThatSpace = "No monster to attack at that space";
        public static string MonsterNotInRangeToAttack = "Monster not in range to attack";
        public static string NotEnoughAttackToAttackMonster = "You do not have enough attack actions to attack this monster";
        public static string MonsterNotInLineOfSightToAttack = "Monster not in line of sight to attack";
    }
}
