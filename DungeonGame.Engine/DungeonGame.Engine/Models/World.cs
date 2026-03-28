using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.GameTemplates;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class World
    {
        public Position HeroPosition { get; set; }
        public Dictionary<SkillType, int> HeroActionPoints { get; set; } = new Dictionary<SkillType, int>();
        public HashSet<Position> Borders { get; set; } = new HashSet<Position>();
        public HashSet<Position> Walls { get; set; } = new HashSet<Position>();
        public List<MonsterPosition> Monsters { get; set; } = new List<MonsterPosition>();

        public void InitializeLevel(int levelNumber, bool initRandomWalls = true)
        {
            // init border
            Walls = new HashSet<Position>();

            for (int i = 0; i < GameConstants.LevelSize + 2; i++)
            {
                Walls.Add(new Position(i, 0));
                Walls.Add(new Position(0, i));
                Walls.Add(new Position(GameConstants.LevelSize + 1, i));
                Walls.Add(new Position(i, GameConstants.LevelSize + 1));
            }

            Borders = new HashSet<Position>(Walls);

            // init level
            var template = LevelTemplates.Levels.Single(x => x.LevelNumber == levelNumber);
            HeroPosition = template.HeroPosition;

            foreach(var wallPosition in template.WallPositions)
            {
                Walls.Add(wallPosition);
            }

            // init random walls
            if (initRandomWalls)
            {
                var numberOfRandomWalls = RandomUtility.RandomInt(template.RandomWallsCountMin, template.RandomWallsCountMax);
                InitializeRandomWalls(numberOfRandomWalls);
            }

            Monsters = new List<MonsterPosition>();
            foreach(var monster in template.MonsterPositions)
            {
                var monsterType = monster.Item1;
                var monsterPosition = monster.Item2;
                
                Monsters.Add(new MonsterPosition() {
                    Monster = MonsterSpawner.Spawn(monsterType), 
                    Position = monsterPosition
                });
            }
        }

        private void InitializeRandomWalls(int numberOfRandomWalls)
        {
            var addedWallsCount = 0;
            var iterations = 0;

            while (addedWallsCount < numberOfRandomWalls)
            {
                var randomPosition = new Position(RandomUtility.RandomInt(1, GameConstants.LevelSize), RandomUtility.RandomInt(1, GameConstants.LevelSize));

                if (!Walls.Contains(randomPosition) 
                    && HeroPosition != randomPosition 
                    && !Monsters.Any(mp => mp.Position == randomPosition))
                {
                    var testWalls = new List<Position>(Walls)
                    {
                        randomPosition
                    };

                    var walkableSquares = GeometryUtility.PlotValuesByFloodSearch(HeroPosition, testWalls, FloodSearchHelpers.WalkValueFunction, 
                            FloodSearchHelpers.FloodUntilAllSquaresWalked, FloodSearchHelpers.ReturnAllPositions);

                    var expectedEmptySquares = (GameConstants.LevelSize + 2) * (GameConstants.LevelSize + 2) // level squares including border
                                        - Walls.Count() // walls and borders
                                        - 1; // subtract 1 for the added wall

                    // check all empty squares can be walked to
                    if (walkableSquares.Count() == expectedEmptySquares)
                    {
                        Walls.Add(randomPosition);
                        addedWallsCount++;
                    }
                }

                iterations++;
                if (iterations >= 100)
                    throw new InvalidOperationException("Could not successfully add random walls after 100 iterations");
            }
        }
    }
}
