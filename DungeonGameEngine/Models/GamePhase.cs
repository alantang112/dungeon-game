namespace DungeonGameEngine.Models
{
    public enum GamePhase
    {
        // Start
        Start,

        // Game
        GameEnergyDicePreRoll,
        EnergyDiceAssignment,
        HeroActions,
        MonstersMove,
        MonstersAttack,
        LevelEnd,
        GameEnd
    }
}

