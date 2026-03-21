using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class MonsterActionsHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.MonsterActions;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] { InputEventType.MonstersMove, InputEventType.MonstersAttack, InputEventType.MonsterActionsEnd };

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.MonstersMove)
            {
                // TODO: PlotValuesByFloodSearch(
                //  1. Seed position with value
                //  2. Blockers
                //  3. ValueFunction: (position, stepNumber, diagonalOrOrthogonalStep, previousValue) => int
                //  4. FloodUntil: (allPositionsWithValues) => int (stepNumber)
                //  5. ReturnPositionsFilter: (position, value, allPositionsWithValues) => bool
                //)

                // For each monster
                //      If monster already at max attack range from hero and in line of sight of hero, continue
                //      Find all possible squares that can be walked to - WalkDistanceFrom (monsters can walk through but not end on monsters)
                //          :PlotValuesByFloodSearch(MonsterPosition, Walls+Hero, () => previousValue + D*3 + O*2), Max(Values) >= MonsterMovement, !monsters.Contains(position) && value <= monsterMovement)
                //          If no walkable squares, continue          
                //      Check if any in range and in line of sight of hero. If yes, find best (max range, then lowest movements required). Move there, end.

                //      Otherwise, find empty squares in attack range from hero with line of sight -> (priority: order by attack range desc)
                //          :PlotValuesByFloodSearch(HeroPosition, levelBorder, () => previousValue + D*3 + O*2) + (has wall ? int.Max : 0) /* filters out walls */ + (!(has line of sight) ? int.Max : 0), Max(Values) >= MonsterAttackRange, !(monsters excluding self).Contains(position) && value <= monsterAttackRange)
                //      Otherwise, find empty squares closest to hero ignoring monsters -> (priority: order by distance to hero asc)
                //          :PlotValuesByFloodSearch(HeroPosition, walls, () => previousValue + D*3 + O*2), positions.Any(p => !(walls+monsters).Contains(p)), value > min(value where position is empty))
                //      For each optimal square, find closest walkable square OR current position
                //          : PlotValuesByFloodSearch(OptimalSquare, walls+hero, () => previousValue + D*3 + O*2), positions.Any(p => target.Contains(p)), target.Contains(position))
                //      Choose walkable square based on walk distance from optimal square. Break tie by movements required, otherwise just get first

                foreach(var monsterPosition in gameState.World.Monsters)
                {
                    if (monsterPosition.Monster.Stats[SkillType.Movement] < GameConstants.MovementPointsOrthogonal)
                        continue;
                    
                    var currentDistanceFromHero = GeometryUtility.CalculateDistanceBetween(monsterPosition.Position, gameState.World.HeroPosition);
                    var currentlyHasLineOfSight = GeometryUtility.HasLineOfSightOf(monsterPosition.Position, gameState.World.HeroPosition, gameState.World.Walls.ToList());

                    // If monster already at max attack range from hero and in line of sight of hero, continue
                    if (currentlyHasLineOfSight && currentDistanceFromHero == monsterPosition.Monster.Stats[SkillType.AttackRange])
                    {
                        continue;
                    }

                    // Find all possible squares that can be walked to
                    var walkableSquares = GetWalkableSquares(monsterPosition, gameState);

                    if (!walkableSquares.Any())
                    {
                        continue;
                    }

                    // Check if any walkable squares are in range and in line of sight of hero
                    var wallsMonsters = new List<Position>();
                    wallsMonsters.AddRange(gameState.World.Walls);
                    wallsMonsters.AddRange(gameState.World.Monsters.Select(mp => mp.Position));

                    var bestWalkableSquareInRangeAndLineOfSight = GetBestPositionInRangeAndLineOfSightOfHero(walkableSquares, gameState, monsterPosition, wallsMonsters);

                    if (bestWalkableSquareInRangeAndLineOfSight != null)
                    {
                        // check if current position is better
                        if (currentlyHasLineOfSight && currentDistanceFromHero >= bestWalkableSquareInRangeAndLineOfSight.Value.DistanceFromHero)
                        {
                            continue;
                        }
                        else 
                        {
                            // TODO: figure out how monster walks there
                            monsterPosition.Position = bestWalkableSquareInRangeAndLineOfSight.Value.Position;
                            continue;
                        }
                    }

                    // Otherwise, try to get as close as possible to max attack range and in line of sight
                    var optimalSquares = new List<Position>();
                    optimalSquares.AddRange(GetBestPositionsInAttackRangeAndLineOfSightOfHero(monsterPosition, gameState));

                    if (!optimalSquares.Any())
                    {
                        // Otherwise, try to get as possible to hero
                        
                    }

                    // Choose walkable square based on walk distance from optimal square
                    // TODO
                }

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.MonstersAttack)
            {
                // Find all monsters in line of sight and in range, add up monster attack, divide by hero defence points, decrease hero health

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.MonsterActionsEnd)
            {
                // Simply move to LevelEnd
            }

            throw new NotImplementedException();
        }

        // TODO: Extract these to another file
        private static Dictionary<Position, int> GetWalkableSquares(MonsterPosition monsterPosition, GameState gameState)
        {
            var walkBlockers = new List<Position>();
            walkBlockers.AddRange(gameState.World.Walls);
            walkBlockers.Add(gameState.World.HeroPosition);

            Func<Dictionary<Position, int>, int?> floodUntilStep = (Dictionary<Position, int> floodValues) =>
            {
                return monsterPosition.Monster.Stats[SkillType.Movement] - 2;
            };

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return value <= monsterPosition.Monster.Stats[SkillType.Movement] && value > 0 && !gameState.World.Monsters.Any(mp => mp.Position == position);
            };

            var walkableSquares = GeometryUtility.PlotValuesByFloodSearch(monsterPosition.Position, walkBlockers, WalkValueFunction, floodUntilStep, returnPositionsFilter);

            return walkableSquares;
        }

        private static CandidateMovementSquare? GetBestPositionInRangeAndLineOfSightOfHero(Dictionary<Position, int> walkableSquares, GameState gameState, MonsterPosition monsterPosition, List<Position> blockers)
        {
            var inRangeAndLineOfSight = walkableSquares
                .Select(walkableSquare => new CandidateMovementSquare
                {
                    Position = walkableSquare.Key,
                    MovementPointsRequired = walkableSquare.Value,
                    DistanceFromHero = GeometryUtility.CalculateDistanceBetween(walkableSquare.Key, gameState.World.HeroPosition)
                })
                .Where(candidate => candidate.DistanceFromHero <= monsterPosition.Monster.Stats[SkillType.AttackRange])
                .Where(candidate => GeometryUtility.HasLineOfSightOf(candidate.Position, gameState.World.HeroPosition, blockers))
                .ToList();

            var optimalSquare = inRangeAndLineOfSight
                .OrderByDescending(x => x.DistanceFromHero)
                .OrderByDescending(x => x.MovementPointsRequired)
                .FirstOrDefault();

            return optimalSquare;
        }

        private static List<Position> GetBestPositionsInAttackRangeAndLineOfSightOfHero(MonsterPosition monsterPosition, GameState gameState)
        {
            var wallsMonstersExcludingSelf = new List<Position>();
            wallsMonstersExcludingSelf.AddRange(gameState.World.Walls);
            wallsMonstersExcludingSelf.AddRange(gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Select(mp => mp.Position));

            Func<Position, int, bool, int, int> valueFunction = (Position position, int stepNumber, bool isDiagonalStep, int previousValue) =>
            {
                if (wallsMonstersExcludingSelf.Contains(position))
                    return int.MaxValue;

                if (!GeometryUtility.HasLineOfSightOf(position, gameState.World.HeroPosition, wallsMonstersExcludingSelf))
                    return int.MaxValue;

                return WalkValueFunction(position, stepNumber, isDiagonalStep, previousValue);
            };

            Func<Dictionary<Position, int>, int?> floodUntilStep = (Dictionary<Position, int> floodValues) =>
            {
                return monsterPosition.Monster.Stats[SkillType.AttackRange] - 2;
            };

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return value <= monsterPosition.Monster.Stats[SkillType.AttackRange] && value > 0 && !gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Any(mp => mp.Position == position);
            };

            var bestPositionsInAttackRangeAndLineOfSight = GeometryUtility.PlotValuesByFloodSearch(gameState.World.HeroPosition, gameState.World.Borders.ToList(), valueFunction, floodUntilStep, returnPositionsFilter);
            
            var maxAttackRange = bestPositionsInAttackRangeAndLineOfSight.Values.Max();

            return bestPositionsInAttackRangeAndLineOfSight.Where(kv => kv.Value == maxAttackRange).Select(x => x.Key).ToList();
        }

        private static int WalkValueFunction(Position position, int stepNumber, bool isDiagonalStep, int previousValue)
        {
            return previousValue + (isDiagonalStep ? GameConstants.MovementPointsDiagonal : GameConstants.MovementPointsOrthogonal);
        }
    }
}

public struct CandidateMovementSquare
{
    public Position Position { get; set; }
    public int MovementPointsRequired { get; set; }
    public int DistanceFromHero { get; set; }
}