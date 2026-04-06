namespace DungeonGame.Engine
{
    public static class GameMessages
    {
        public static string YouHaveEnteredLevel = "{0} has entered level {1}";
        public static string DiceRolled = "Energy dice rolled: {0}, {1}, {2}";
        public static string DiceAssignedToSkill = "Energy dice {0} assigned to {1}";
        public static string SkillAlreadyAssignedEnergyDice = "This skill type is already assigned an energy dice";
        public static string InvalidSkillForEnergyDiceAssignment = "This skill type cannot be assigned an energy dice";
        public static string AssignAllEnergyDiceBeforeProceeding = "Please assign all energy dice before proceeding";
        public static string HeroMovedTo = "{0} moves to ({1},{2})";
        public static string NotEnoughMovementActionPoints = "{0} does not have any more energy to move";
        public static string CanOnlyMoveAdjacently = "{0} can only move to adjacent spaces";
        public static string CannotMoveToThatSpace = "{0} cannot move to that space";
        public static string HeroAttacksMonster = "{0} attacks {1} {3}! It has {2} health point(s) remaining";
        public static string NoMonsterToAttackAtThatSpace = "No monster to attack at that space";
        public static string MonsterNotInRangeToAttack = "{0} {1} not in range to attack";
        public static string NotEnoughAttackToAttackMonster = "{0} does not have enough strength to attack {1} {2} at this time";
        public static string MonsterNotInLineOfSightToAttack = "{0} {1} not in line of sight to attack";
        public static string MonsterDefeated = "{0} {1} has been slain!";
        public static string AllMonstersDefeated = "All monsters on this level has been defeated!";
        public static string HeroReset = "{0} rethinks {1} life choices...";
        public static string MonsterMoves = "{0} {3} moves to ({1},{2}))";
        public static string MonsterStays = "{0} {1} stands ready";
        public static string MonstersAttack = "The monsters attack {0} with {1} attack point(s) against your {2} point(s) of defence, dealing {3} point(s) of damage!";
        public static string HeroDefeated = "{0} has been slain. R.I.P {1:dd.MM.yyyy}-{2:dd.MM.yyyy}";
        public static string MonsterAttackAvoided = "{0} has avoided the monsters for now";
        public static string LevelUpError = "You must choose to level up a skill or replenish health but not both";
        public static string LevelUpReplenishHealth = "{0} takes a long rest and is filled with newfound ENERGY";
        public static string LevelUpSkill = "{0} has improved {1}";
        public static string NoRerollsAvailable = "No rerolls available";
    }
}
