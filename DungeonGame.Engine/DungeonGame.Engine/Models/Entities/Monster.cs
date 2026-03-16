using DungeonGame.Engine.Models.Enums;

namespace DungeonGame.Engine.Models.Entities
{
    public abstract class Monster
    {
        public abstract MonsterType Type { get; }
    }
}
