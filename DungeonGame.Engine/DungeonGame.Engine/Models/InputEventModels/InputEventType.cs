namespace DungeonGame.Engine.Models.InputEventModels
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
        HeroActionConfirm,

        // Game actions - Monsters
        MonsterTurnContinue,

        // Game actions - LevelEnd
        LevelEndChooseUpgrade,

        // Game actions - GameEnd
    }
}
