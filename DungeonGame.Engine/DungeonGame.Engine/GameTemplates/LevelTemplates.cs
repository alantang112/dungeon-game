using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.GameTemplates
{
    public static class LevelTemplates
    {
        public static Level[] Levels = new Level[]
        {
            new Level()
            {
                LevelNumber = 1,
                HeroPosition = new Position(1, 1),
                WallPositions = new Position[] { new Position(2, 2), new Position(4, 2), new Position(4, 4) }, 
                RandomWallsCountMin = 2,
                RandomWallsCountMax = 2,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Spider, new Position(5,4)), (MonsterType.Spider, new Position(4,5)) }
            },
            new Level()
            {
                LevelNumber = 2,
                HeroPosition = new Position(1, 5),
                WallPositions = new Position[] { new Position(1, 3), new Position(4, 2), new Position(4, 3) }, 
                RandomWallsCountMin = 1,
                RandomWallsCountMax = 2,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Spider, new Position(2,3)), (MonsterType.Spider, new Position(5,2)) }
            }
        };
    }
}
