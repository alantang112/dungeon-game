using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.GameInputHandlers.Handlers;
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
        public int RerollsAvailable { get; set; } = 0;
        public HashSet<Position> Borders { get; set; } = new HashSet<Position>();
        public HashSet<Position> Walls { get; set; } = new HashSet<Position>();
        public List<MonsterPosition> Monsters { get; set; } = new List<MonsterPosition>();

        private static HashSet<Position> GenerateBorders()
        {
            var borders = new HashSet<Position>();

            for (int i = 0; i < GameConstants.LevelSize + 2; i++)
            {
                borders.Add(new Position(i, 0));
                borders.Add(new Position(0, i));
                borders.Add(new Position(GameConstants.LevelSize + 1, i));
                borders.Add(new Position(i, GameConstants.LevelSize + 1));
            }

            return borders;
        }

        /// <summary>
        /// Creates random walls within the level. Should be run after hero and monsters are spawned.
        /// </summary>
        /// <param name="numberOfRandomWalls"></param>
        /// <param name="enforceWallIslands"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void InitializeRandomWalls(int numberOfRandomWalls, bool enforceWallIslands)
        {
            var addedWallsCount = 0;

            var candidates = Enumerable.Range(1, GameConstants.LevelSize).SelectMany(x => Enumerable.Range(1, GameConstants.LevelSize), (x, y) => new Position(x, y))
                .Where(x => !Walls.Contains(x) && HeroPosition != x && !Monsters.Any(mp => mp.Position == x))
                .ToList();

            while (addedWallsCount < numberOfRandomWalls)
            {
                var candidate = candidates.OrderBy(_ => Guid.NewGuid()).First();

                var testWalls = new List<Position>(Walls)
                {
                    candidate
                };

                var walkableSquares = GeometryUtility.PlotValuesByFloodSearch(HeroPosition, testWalls, FloodSearchHelpers.WalkValueFunction, 
                        FloodSearchHelpers.FloodUntilAllSquaresWalked, FloodSearchHelpers.ReturnAllValidPositions);

                var expectedEmptySquares = (GameConstants.LevelSize + 2) * (GameConstants.LevelSize + 2) // level squares including border
                                    - Walls.Count() // walls and borders
                                    - 1; // subtract 1 for the added wall

                // check all empty squares can be walked to. If needs wall islands, check there are wall islands
                if (walkableSquares.Count() == expectedEmptySquares && (!enforceWallIslands || GeometryUtility.HasWallIsland(testWalls)))
                {
                    Walls.Add(candidate);
                    addedWallsCount++;
                }

                candidates.Remove(candidate);

                if (!candidates.Any())
                    throw new InvalidOperationException("Could not successfully add random walls");
            }
        }
    
        public List<Position> CalculateHeroCanWalkPositions()
        {
            var result = new List<Position>();

            foreach(var checkPosition in GeometryUtility.GetNeighbouringPositions(HeroPosition))
            {
                var (caWalk, _) = HeroActionsHandler.HeroCanWalkTo(this, checkPosition);

                if (caWalk)
                    result.Add(checkPosition);
            }
            
            return result;
        }

        public List<Position> CalculateHeroCanAttackPositions(int heroAttackRange)
        {
            var result = new List<Position>();

            var wallsAndMonsters = new List<Position>();
            wallsAndMonsters.AddRange(Walls);
            wallsAndMonsters.AddRange(Monsters.Select(mp => mp.Position));

            foreach(var monsterPosition in Monsters)
            {
                if (HeroActionPoints[SkillType.Attack] < monsterPosition.Monster.GetStat(SkillType.Defence))
                    continue;

                if (heroAttackRange < GeometryUtility.CalculateDistanceBetween(HeroPosition, monsterPosition.Position))
                    continue;

                if (!GeometryUtility.HasLineOfSightOf(HeroPosition, monsterPosition.Position, wallsAndMonsters))
                    continue;

                result.Add(monsterPosition.Position);
            }

            return result;
        }

        public void InitializeWallBorder()
        {
            Walls = new HashSet<Position>();

            if (!Borders.Any())
                Borders = GenerateBorders();

            Walls.UnionWith(Borders); 
        }

        public List<Position> FindSpawnPositions(int numberOfPositionsRequired, int? numberOfPositionsToChooseFrom = null)
        {
            numberOfPositionsToChooseFrom ??= (numberOfPositionsRequired * 2);

            var walkDistanceFromHeroMap = GeometryUtility.PlotValuesByFloodSearch(
                HeroPosition,
                Walls.ToList(),
                FloodSearchHelpers.WalkValueFunction,
                FloodSearchHelpers.FloodUntilAllSquaresWalked,
                FloodSearchHelpers.ReturnAllValidPositions
            );

            foreach(var monsterPosition in Monsters)
            {
                if (walkDistanceFromHeroMap.ContainsKey(monsterPosition.Position))
                    walkDistanceFromHeroMap.Remove(monsterPosition.Position);
            }

            var numberOfPositions = walkDistanceFromHeroMap.Count();

            var result = walkDistanceFromHeroMap
                    .OrderByDescending(x => x.Value)
                    .Take(Math.Min(numberOfPositionsToChooseFrom.Value, numberOfPositions))
                    .Select(x => x.Key)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(numberOfPositionsRequired)
                    .ToList();
            
            return result;
        }
    }
}
