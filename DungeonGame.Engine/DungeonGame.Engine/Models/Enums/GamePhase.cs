namespace DungeonGame.Engine.Models.Enums
{
    public enum GamePhase
    {
        // Start
        Start,

        // Game
        EnergyDicePreRoll,
        EnergyDiceAssignment,
        HeroActions,
        MonstersMove,
        MonstersAttack,
        LevelEnd,
        GameEnd
    }
}

