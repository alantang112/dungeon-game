namespace DungeonGame.Engine.Models.Enums
{
    public enum InputEventType
    {
        // Start actions
        NewGame,
        
        // Game actions - EnergyDice
        EnergyDiceRoll,
        EnergyDiceAssign,
        EnergyDiceResetAssignment,
        EnergyDiceConfirmAssignment,

        // Game actions - Hero
        HeroActionMove,
        HeroActionAttack,
        HeroActionReset,
        HeroActionEnd, // will trigger monster actions

        // Game actions - Monsters
        MonstersMove,
        MonstersAttack,
        MonsterActionsEnd,

        // Game actions - LevelEnd
        LevelEnd,

        // Game actions - GameEnd
    }
}
