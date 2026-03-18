using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public static class LevelDefinitions
    {
        public static Level[] Levels = new Level[]
        {
            new Level()
            {
                LevelNumber = 1,
                HeroPosition = new Position(1, 1),
                WallPositions = new Position[] { new Position(2, 2), new Position(4, 2), new Position(4, 4) }, 
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Spider, new Position(5,4)), (MonsterType.Spider, new Position(4,5)) }
            }
        };
    }
}
