using System;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.GameContent
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
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Spider, new Position(5, 4)), (MonsterType.Spider, new Position(4, 5)) }
            },
            new Level()
            {
                LevelNumber = 2,
                HeroPosition = new Position(1, 5),
                WallPositions = new Position[] { new Position(1, 3), new Position(4, 2), new Position(4, 3) }, 
                RandomWallsCountMin = 1,
                RandomWallsCountMax = 1,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Skeleton, new Position(2, 3)), (MonsterType.Skeleton, new Position(5, 2)), (MonsterType.Skeleton, new Position(1, 2)) }
            },
            new Level()
            {
                LevelNumber = 3,
                HeroPosition = new Position(5, 1),
                WallPositions = new Position[] { new Position(2, 2), new Position(2, 4), new Position(4, 4) }, 
                RandomWallsCountMin = 3,
                RandomWallsCountMax = 3,
                EnforceWallIslands = true,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Minotaur, new Position(1, 1)) }
            },
            new Level()
            {
                LevelNumber = 4,
                HeroPosition = new Position(3, 3),
                WallPositions = new Position[] { new Position(2, 4), new Position(2, 3), new Position(5, 3 ) },
                RandomWallsCountMin = 2,
                RandomWallsCountMax = 3,
                EnforceWallIslands = true,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Fiendling, new Position(2, 5)) }
            },
            new Level()
            {
                LevelNumber = 5,
                HeroPosition = new Position(1, 1),
                WallPositions = new Position[] { new Position(2, 2), new Position(4, 2), new Position(3, 4) },
                RandomWallsCountMin = 1,
                RandomWallsCountMax = 1,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Spider, new Position(2, 5)), (MonsterType.Spider, new Position(5, 4)), (MonsterType.Spider, new Position(5, 2)) }
            },
            new Level()
            {
                LevelNumber = 6,
                HeroPosition = new Position(1, 5),
                WallPositions = new Position[] { new Position(2, 3), new Position(4, 2), new Position(4, 3) },
                RandomWallsCountMin = 2,
                RandomWallsCountMax = 2,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Skeleton, new Position(4, 5)), (MonsterType.Skeleton, new Position(5, 4)), (MonsterType.Skeleton, new Position(1, 1)), (MonsterType.Spider, new Position(5, 5)) }
            },
            new Level()
            {
                LevelNumber = 7,
                HeroPosition = new Position(5, 2),
                WallPositions = new Position[] { new Position(2, 3), new Position(3, 3), new Position(4, 3 ) },
                RandomWallsCountMin = 1,
                RandomWallsCountMax = 2,
                EnforceWallIslands = true,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Minotaur, new Position(1, 1)), (MonsterType.Minotaur, new Position(3, 5)) }
            },
            new Level()
            {
                LevelNumber = 8,
                HeroPosition = new Position(1, 3),
                WallPositions = new Position[] { new Position(2, 2), new Position(4, 4) },
                RandomWallsCountMin = 4,
                RandomWallsCountMax = 4,
                EnforceWallIslands = true,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Colossus, new Position(5, 3)) }
            },
            new Level()
            {
                LevelNumber = 9,
                HeroPosition = new Position(3, 3),
                WallPositions = new Position[] { new Position(3, 2), new Position(3, 4) },
                RandomWallsCountMin = 4,
                RandomWallsCountMax = 4,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Fiendling, new Position(1, 1)), (MonsterType.Fiendling, new Position(5, 5)), 
                    (MonsterType.Skeleton, new Position(5, 1)), (MonsterType.Skeleton, new Position(1, 5)) }
            },
            new Level()
            {
                LevelNumber = 10,
                HeroPosition = new Position(1, 2),
                WallPositions = new Position[] { new Position(1, 1), new Position(1, 5), new Position(5, 1), new Position(5, 5), new Position(3, 3)},
                RandomWallsCountMin = 0,
                RandomWallsCountMax = 0,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Overseer, new Position(5, 4))}
            },
            new Level()
            {
                LevelNumber = 11,
                HeroPosition = new Position(3, 1),
                WallPositions = new Position[] { new Position(3, 3), new Position(1, 4), new Position(4, 1) },
                RandomWallsCountMin = 0,
                RandomWallsCountMax = 0,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Direwolf, new Position(1, 5)), (MonsterType.Direwolf, new Position(5, 5)) }
            },
            new Level()
            {
                LevelNumber = 12,
                HeroPosition = new Position(4, 4),
                WallPositions = new Position[] { new Position(3, 3), new Position(2, 3), },
                RandomWallsCountMin = 3,
                RandomWallsCountMax = 3,
                EnforceWallIslands = true,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Colossus, new Position(1, 1)), (MonsterType.Colossus, new Position(5, 1)), (MonsterType.Colossus, new Position(1, 4)) }
            },
            new Level()
            {
                LevelNumber = 13,
                HeroPosition = new Position(2, 2),
                WallPositions = new Position[] { new Position(1, 1), new Position(5, 1), new Position(5, 5), new Position(1, 5), },
                RandomWallsCountMin = 1,
                RandomWallsCountMax = 1,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Reaper, new Position(3, 4)), (MonsterType.Reaper, new Position(5, 2)) }
            },
            new Level()
            {
                LevelNumber = 14,
                HeroPosition = new Position(1, 2),
                WallPositions = new Position[] { new Position(3, 3), },
                RandomWallsCountMin = 3,
                RandomWallsCountMax = 3,
                MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Direwolf, new Position(3, 5)), (MonsterType.Direwolf, new Position(5, 3)), (MonsterType.Direwolf, new Position(4, 1)) }
            },
            new Level()
            {
              LevelNumber = 15,
              HeroPosition = new Position(1, 5),
              WallPositions = new Position[] { new Position(1,1), new Position(3,3), new Position(5,5)},
              MonsterPositions = new (MonsterType, Position)[] { (MonsterType.Oathbound, new Position(3, 4)), (MonsterType.Elfling, new Position(5, 1)) }  
            },
            // Test levels
            new Level()
            {
                LevelNumber = -1,  
                HeroPosition = new Position(1, 1),
                WallPositions = Array.Empty<Position>(), 
                RandomWallsCountMin = 0,
                RandomWallsCountMax = 0,
                MonsterPositions = Array.Empty<(MonsterType, Position)>()
            },
        };
    }
}
