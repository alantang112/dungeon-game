namespace DungeonGame.Engine
{
    public static class GameMessages
    {
        public static string YouHaveEnteredLevel = "You have entered level {0}";
        public static string DiceRolled = "Energy dice rolled";
        public static string DiceAssignedToSkill = "Energy dice {0} assigned to {1}";
        public static string DiceAssignmentReset = "Energy dice assignment reset";
        public static string DiceAssignmentConfirmed = "Energy dice assignment confirmed";
        public static string SkillAlreadyAssignedEnergyDice = "This skill type is already assigned an energy dice";
        public static string InvalidSkillForEnergyDiceAssignment = "This skill type cannot be assigned an energy dice";
        public static string AssignAllEnergyDiceBeforeProceeding = "Please assign all energy dice before proceeding";
        public static string HeroMovedTo = "{0} moved to ({1},{2})";
        public static string NotEnoughMovementActionPoints = "You do not have enough movement action points";
        public static string CanOnlyMoveAdjacently = "You can only move to adjacent spaces";
        public static string CannotMoveToThatSpace = "You cannot move to that space";
        public static string HeroAttacksMonster = "{0} attacks {1} at ({2},{3})";
        public static string NoMonsterToAttackAtThatSpace = "No monster to attack at that space";
        public static string MonsterNotInRangeToAttack = "Monster not in range to attack";
        public static string NotEnoughAttackToAttackMonster = "You do not have enough attack actions to attack this monster";
        public static string MonsterNotInLineOfSightToAttack = "Monster not in line of sight to attack";
        public static string MonsterDefeated = "The {0} has been slain!";
        public static string AllMonstersDefeated = "All monsters on this level has been defeated!";
        public static string HeroTurnReset = "{0} rethinks their life choices...";
        public static string MonsterMoves = "{0} at ({1},{2}) moves to ({3},{4})";
        public static string MonstersAttack = "The monsters attack you with {0} attack points against your {2} points of defence, dealing {1} points of damage!";
        public static string HeroDefeated = "{0} has been slain";
    }
}
