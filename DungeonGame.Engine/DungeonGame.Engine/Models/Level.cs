using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public class Level
    {
        public int LevelNumber { get; set; }
        public Position HeroPosition { get; set; }
        public Position[] WallPositions { get; set; }
        public (MonsterType, Position)[] MonsterPositions { get; set; }
    }
}
