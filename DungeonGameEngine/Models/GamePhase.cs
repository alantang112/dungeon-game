namespace DungeonGameEngine.Models
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

