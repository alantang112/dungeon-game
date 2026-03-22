namespace DungeonGame.Engine
{
    public static class GameMessages
    {
        public static string YouHaveEnteredLevel = "{0} has entered level {0}";
        public static string DiceRolled = "Energy dice rolled: {0}, {1}, {2}";
        public static string DiceAssignedToSkill = "Energy dice {0} assigned to {1}";
        public static string DiceAssignmentConfirmed = "Energy dice assignment confirmed";
        public static string SkillAlreadyAssignedEnergyDice = "This skill type is already assigned an energy dice";
        public static string InvalidSkillForEnergyDiceAssignment = "This skill type cannot be assigned an energy dice";
        public static string AssignAllEnergyDiceBeforeProceeding = "Please assign all energy dice before proceeding";
        public static string HeroMovedTo = "{0} moves to ({1},{2})";
        public static string NotEnoughMovementActionPoints = "{0} does not have any more energy to move";
        public static string CanOnlyMoveAdjacently = "{0} can only move to adjacent spaces";
        public static string CannotMoveToThatSpace = "{0} cannot move to that space";
        public static string HeroAttacksMonster = "{0} attacks {1} at ({2},{3})";
        public static string NoMonsterToAttackAtThatSpace = "No monster to attack at that space";
        public static string MonsterNotInRangeToAttack = "Monster not in range to attack";
        public static string NotEnoughAttackToAttackMonster = "{0} does not have enough strength to attack this monster at this time";
        public static string MonsterNotInLineOfSightToAttack = "Monster not in line of sight to attack";
        public static string MonsterDefeated = "The {0} has been slain!";
        public static string AllMonstersDefeated = "All monsters on this level has been defeated!";
        public static string HeroReset = "{0} rethinks their life choices...";
        public static string MonsterMoves = "{0} at ({1},{2}) moves to ({3},{4})";
        public static string MonsterStays = "{0} at ({1},{2}) stands ready";
        public static string MonstersAttack = "The monsters attack {0} with {1} attack points against your {2} points of defence, dealing {3} points of damage!";
        public static string HeroDefeated = "{0} has been slain. R.I.P {1}-{2}";
        public static string MonsterAttackAvoided = "{0} has avoided the monsters for now";
    }
}
