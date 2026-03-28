using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public class Level
    {
        public int LevelNumber { get; set; }
        public Position HeroPosition { get; set; }
        public Position[] WallPositions { get; set; }
        public int RandomWallsCountMin { get; set; } = 0;
        public int RandomWallsCountMax { get; set; } = 0;
        public (MonsterType, Position)[] MonsterPositions { get; set; }
    }
}
