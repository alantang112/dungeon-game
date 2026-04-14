namespace DungeonGame.Engine.Models.Enums
{
    public enum InputEventType
    {
        // Start actions
        NewGame,
        
        // Game actions - UpgradeHero
        UpgradeHeroSetup,
        UpgradeHero,

        // Game actions - EnergyDice
        EnergyDiceSetup,
        EnergyDiceRoll,
        EnergyDiceAssign,
        EnergyDiceResetAssignment,
        EnergyDiceReroll,

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
        NextLevel,

        // Game actions - GameEnd
        RetryLevel,
        BackToStart // TODO
    }
}
