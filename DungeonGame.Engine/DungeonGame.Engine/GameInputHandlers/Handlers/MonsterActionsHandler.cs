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
                return PerformMonsterMove(gameState);
            }
            else if (inputEvent.EventType == InputEventType.MonstersAttack)
            {
                return PerformMonsterAttack(gameState);
            }
            else if (inputEvent.EventType == InputEventType.MonsterActionsEnd)
            {
                // TODO: unit test this
                gameState.World.Monsters.ForEach(mp =>
                {
                    if (mp.Monster.IsBossType)
                    {
                        mp.Monster.BossDice.Clear();
                    }
                });

                gameState.GamePhase = GamePhase.EnergyDicePreRoll;

                gameState.ScheduledEvents.Add(new InputEvent()
                {
                    EventType = InputEventType.EnergyDiceSetup
                });

                return gameState;
            }

            throw new NotImplementedException();
        }

        private static GameState PerformMonsterAttack(GameState gameState)
        {
            var wallsAndMonsters = new List<Position>();
            wallsAndMonsters.AddRange(gameState.World.Walls);
            wallsAndMonsters.AddRange(gameState.World.Monsters.Select(mp => mp.Position));

            // Find all monsters in line of sight and in range, add up monster attack, divide by hero defence points, decrease hero health
            var totalMonsterAttack = 0;
            foreach(var monsterPosition in gameState.World.Monsters)
            {
                if (GeometryUtility.CalculateDistanceBetween(monsterPosition.Position, gameState.World.HeroPosition) <= monsterPosition.Monster.GetStat(SkillType.AttackRange)
                        && GeometryUtility.HasLineOfSightOf(monsterPosition.Position, gameState.World.HeroPosition, wallsAndMonsters))
                {
                    totalMonsterAttack += monsterPosition.Monster.GetStat(SkillType.Attack);
                }
            }

            if (totalMonsterAttack > 0)
            {
                var damageDealt = (int) Math.Floor((double)totalMonsterAttack / gameState.World.HeroActionPoints[SkillType.Defence]);

                gameState.Hero.Health -= damageDealt;
                gameState.AddGameMessage(string.Format(GameMessages.MonstersAttack, gameState.Hero.Name, totalMonsterAttack, gameState.World.HeroActionPoints[SkillType.Defence], damageDealt));
                if (gameState.Hero.Health <= 0)
                {
                    gameState.GamePhase = GamePhase.GameEnd;
                    gameState.AddGameMessage(string.Format(GameMessages.HeroDefeated, gameState.Hero.Name, gameState.Hero.BirthYear, DateTime.Now.Year));
                }
            }
            else
            {
                gameState.AddGameMessage(string.Format(GameMessages.MonsterAttackAvoided, gameState.Hero.Name));
            }

            return gameState;
        }

        #region MovementHelpers
        private static GameState PerformMonsterMove(GameState gameState)
        {
            var walkDistanceFromHeroMap = GeometryUtility.PlotValuesByFloodSearch(
                gameState.World.HeroPosition,
                gameState.World.Walls.ToList(),
                FloodSearchHelpers.WalkValueFunction,
                FloodSearchHelpers.FloodUntilAllSquaresWalked,
                FloodSearchHelpers.ReturnAllPositions
            );

            foreach(var monsterPosition in gameState.World.Monsters.OrderBy(x => walkDistanceFromHeroMap[x.Position]))
            {
                monsterPosition.LastMovementPath.Clear();
                var monsterOriginalPosition = monsterPosition.Position;
                
                var monsterDoesNotMoveMessage = string.Format(GameMessages.MonsterStays, monsterPosition.Monster.Type, monsterPosition.Monster.Name);

                if (monsterPosition.Monster.GetStat(SkillType.Movement) < GameConstants.MovementPointsOrthogonal)
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }
                    
                var currentDistanceFromHero = GeometryUtility.CalculateDistanceBetween(monsterPosition.Position, gameState.World.HeroPosition);

                var wallsAndMonstersExcludingSelf = GetWallsAndMonstersExcludingSelf(monsterPosition, gameState);
                var currentlyHasLineOfSight = GeometryUtility.HasLineOfSightOf(monsterPosition.Position, gameState.World.HeroPosition, wallsAndMonstersExcludingSelf);

                // If monster already at max attack range from hero and in line of sight of hero, continue
                if (currentlyHasLineOfSight && currentDistanceFromHero == monsterPosition.Monster.GetStat(SkillType.AttackRange))
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }

                // Find all possible positions that can be walked to
                var walkablePositionsIncludingMonsters = GetWalkablePositions(monsterPosition, gameState);
                var walkablePositions = walkablePositionsIncludingMonsters.Where(x => !gameState.World.Monsters.Any(mp => mp.Position == x.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);

                if (!walkablePositions.Any())
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }

                // Check if any walkable positions are in range and in line of sight of hero
                var bestWalkablePositionInRangeAndLineOfSight = GetBestPositionInRangeAndLineOfSightOfHero(walkablePositions, gameState, monsterPosition);

                if (bestWalkablePositionInRangeAndLineOfSight != null)
                {
                    // check if current position is better
                    if (currentlyHasLineOfSight && currentDistanceFromHero <= monsterPosition.Monster.GetStat(SkillType.AttackRange) && currentDistanceFromHero >= bestWalkablePositionInRangeAndLineOfSight.DistanceFromTarget)
                    {
                        gameState.AddGameMessage(monsterDoesNotMoveMessage);
                        continue;
                    }
                    else 
                    {
                        monsterPosition.LastMovementPath = GeometryUtility.FindWalkPath(walkablePositionsIncludingMonsters, monsterPosition.Position, bestWalkablePositionInRangeAndLineOfSight.Position);
                        monsterPosition.Position = bestWalkablePositionInRangeAndLineOfSight.Position;
                        gameState.AddGameMessage(string.Format(GameMessages.MonsterMoves, monsterPosition.Monster.Type, monsterPosition.Position.X, monsterPosition.Position.Y, monsterPosition.Monster.Name));
                        continue;
                    }
                }

                CandidateMovementPosition? movementCandidate = null;

                // Otherwise, try to get as close as possible to max attack range and in line of sight
                var optimalPositions = GetBestPositionsInAttackRangeAndLineOfSightOfHero(monsterPosition, gameState);

                if (optimalPositions.Any())
                {
                    movementCandidate = GetBestWalkableCandidateFromOptimalPositions(walkablePositions, monsterPosition, gameState, optimalPositions);
                }

                if (movementCandidate == null || movementCandidate.DistanceFromTarget == int.MaxValue)
                {
                    // Otherwise, try to get as close as possible to hero
                    movementCandidate = GetBestWalkableCandidateClosestToHero(walkablePositions, monsterPosition.Position, walkDistanceFromHeroMap);
                }

                // If moving would put monster out of attack range of hero, do not move
                if (currentlyHasLineOfSight 
                    && currentDistanceFromHero <= monsterPosition.Monster.GetStat(SkillType.AttackRange) 
                    && (!GeometryUtility.HasLineOfSightOf(movementCandidate.Position, gameState.World.HeroPosition, wallsAndMonstersExcludingSelf) || movementCandidate.DistanceFromTarget > monsterPosition.Monster.GetStat(SkillType.AttackRange)))
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }

                if (movementCandidate.Position != monsterPosition.Position)
                {
                    monsterPosition.LastMovementPath = GeometryUtility.FindWalkPath(walkablePositionsIncludingMonsters, monsterPosition.Position, movementCandidate.Position);
                    monsterPosition.Position = movementCandidate.Position;
                    gameState.AddGameMessage(string.Format(GameMessages.MonsterMoves, monsterPosition.Monster.Type, monsterPosition.Position.X, monsterPosition.Position.Y, monsterPosition.Monster.Name));
                    continue;
                }

                gameState.AddGameMessage(monsterDoesNotMoveMessage);
            }

            return gameState;
        }

        private static Dictionary<Position, int> GetWalkablePositions(MonsterPosition monsterPosition, GameState gameState)
        {
            var walkBlockers = GetWalkBlockers(gameState);

            Func<Dictionary<Position, int>, int?> floodUntilStep = (Dictionary<Position, int> floodValues) =>
            {
                return monsterPosition.Monster.GetStat(SkillType.Movement) - 2;
            };

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return value <= monsterPosition.Monster.GetStat(SkillType.Movement) && value > 0;
            };

            var walkablePositions = GeometryUtility.PlotValuesByFloodSearch(monsterPosition.Position, walkBlockers, FloodSearchHelpers.WalkValueFunction, floodUntilStep, returnPositionsFilter);

            return walkablePositions;
        }

        private static List<Position> GetWalkBlockers(GameState gameState)
        {
            var walkBlockers = new List<Position>();
            walkBlockers.AddRange(gameState.World.Walls);
            walkBlockers.Add(gameState.World.HeroPosition);

            return walkBlockers;
        }

        private static CandidateMovementPosition? GetBestPositionInRangeAndLineOfSightOfHero(Dictionary<Position, int> walkablePositions, GameState gameState, MonsterPosition monsterPosition)
        {
            var wallsMonsters = new List<Position>();
            wallsMonsters.AddRange(gameState.World.Walls);
            wallsMonsters.AddRange(gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Select(mp => mp.Position));

            var walkablePositionsCalculation = walkablePositions
                .Select(walkablePosition => new CandidateMovementPosition
                {
                    Position = walkablePosition.Key,
                    MovementPointsRequired = walkablePosition.Value,
                    DistanceFromTarget = GeometryUtility.CalculateDistanceBetween(walkablePosition.Key, gameState.World.HeroPosition)
                })
                .ToList();

            var inRangeAndLineOfSight = walkablePositionsCalculation
                .Where(candidate => candidate.DistanceFromTarget <= monsterPosition.Monster.GetStat(SkillType.AttackRange))
                .Where(candidate => GeometryUtility.HasLineOfSightOf(candidate.Position, gameState.World.HeroPosition, wallsMonsters))
                .ToList();

            if (!inRangeAndLineOfSight.Any())
                return null;

            var optimalPosition = inRangeAndLineOfSight
                .OrderByDescending(x => x.DistanceFromTarget)
                .ThenBy(x => x.MovementPointsRequired)
                .FirstOrDefault();

            return optimalPosition;
        }

        private static List<Position> GetWallsAndMonstersExcludingSelf(MonsterPosition monsterPosition, GameState gameState)
        {
            var result = new List<Position>();
            result.AddRange(gameState.World.Walls);
            result.AddRange(gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Select(mp => mp.Position));

            return result;
        }

        private static List<Position> GetBestPositionsInAttackRangeAndLineOfSightOfHero(MonsterPosition monsterPosition, GameState gameState)
        {
            var wallsMonstersExcludingSelf = GetWallsAndMonstersExcludingSelf(monsterPosition, gameState);

            Func<Position, int, bool, int, int> valueFunction = (Position position, int stepNumber, bool isDiagonalStep, int previousValue) =>
            {
                if (wallsMonstersExcludingSelf.Contains(position))
                    return int.MaxValue;

                if (!GeometryUtility.HasLineOfSightOf(position, gameState.World.HeroPosition, wallsMonstersExcludingSelf))
                    return int.MaxValue;

                return FloodSearchHelpers.WalkValueFunction(position, stepNumber, isDiagonalStep, previousValue);
            };

            Func<Dictionary<Position, int>, int?> floodUntilStep = (Dictionary<Position, int> floodValues) =>
            {
                return monsterPosition.Monster.GetStat(SkillType.AttackRange) - 2;
            };

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return value <= monsterPosition.Monster.GetStat(SkillType.AttackRange) && value > 0 && !gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Any(mp => mp.Position == position);
            };

            var positionsInAttackRangeAndLineOfSight = GeometryUtility.PlotValuesByFloodSearch(gameState.World.HeroPosition, gameState.World.Borders.ToList(), valueFunction, floodUntilStep, returnPositionsFilter);
            
            if (!positionsInAttackRangeAndLineOfSight.Any())
                return new List<Position>();

            var maxAttackRange = positionsInAttackRangeAndLineOfSight.Values.Max();

            return positionsInAttackRangeAndLineOfSight.Where(kv => kv.Value == maxAttackRange).Select(x => x.Key).ToList();
        }

        private static CandidateMovementPosition GetBestWalkableCandidateClosestToHero(Dictionary<Position, int> walkablePositions, Position currentPosition, Dictionary<Position, int> walkDistanceFromHeroMap)
        {
            var candidates = new List<CandidateMovementPosition>
            {
                new CandidateMovementPosition()
                {
                    Position = currentPosition,
                    MovementPointsRequired = 0,
                    DistanceFromTarget = walkDistanceFromHeroMap[currentPosition]
                }
            };

            foreach(var walkablePosition in walkablePositions)
            {
                candidates.Add(new CandidateMovementPosition()
                {
                    Position = walkablePosition.Key,
                    MovementPointsRequired = walkablePosition.Value,
                    DistanceFromTarget = walkDistanceFromHeroMap[walkablePosition.Key]
                });
            }

            var minDistanceFromTarget = candidates.Min(x => x.DistanceFromTarget);

            return candidates.Where(x => x.DistanceFromTarget == minDistanceFromTarget)
                             .OrderBy(x => x.MovementPointsRequired)
                             .First();
        }

        private static CandidateMovementPosition GetBestWalkableCandidateFromOptimalPositions(Dictionary<Position, int> walkablePositions, MonsterPosition monsterPosition, GameState gameState, List<Position> optimalPositions)
        {
            // Choose (walkable position OR current position) based on walk distance from optimal position
            var walkablePositionCandidates = new List<CandidateMovementPosition>();
            walkablePositionCandidates.AddRange(walkablePositions.Select(x => new CandidateMovementPosition()
            {
                Position = x.Key,
                MovementPointsRequired = x.Value,
                DistanceFromTarget = int.MaxValue
            }));

            walkablePositionCandidates.Add(new CandidateMovementPosition()
            {
                Position = monsterPosition.Position,
                MovementPointsRequired = 0,
                DistanceFromTarget = int.MaxValue
            });

            var walkBlockers = GetWalkBlockers(gameState);
            Func<Dictionary<Position, int>, int?> floodUntilStep = (Dictionary<Position, int> floodValues) =>
            {
                return floodValues.Any(fv => walkablePositionCandidates.Any(c => c.Position == fv.Key)) ? (int?)0 : null; // end on next step if reached a target
            };

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return walkablePositionCandidates.Any(x => x.Position == position);
            };

            foreach(var optimalPosition in optimalPositions)
            {
                var distanceToOptimalPositions = GeometryUtility.PlotValuesByFloodSearch(optimalPosition, walkBlockers, FloodSearchHelpers.WalkValueFunction, floodUntilStep, returnPositionsFilter);

                foreach(var distanceToOptimalPosition in distanceToOptimalPositions)
                {
                    var walkablePositionCandidate = walkablePositionCandidates.Single(x => x.Position == distanceToOptimalPosition.Key);

                    if (distanceToOptimalPosition.Value < walkablePositionCandidate.DistanceFromTarget)
                    {
                        walkablePositionCandidate.DistanceFromTarget = distanceToOptimalPosition.Value;
                    }
                }
            }

            var bestWalkablePositionCandidate = walkablePositionCandidates
                .OrderBy(x => x.DistanceFromTarget)
                .ThenBy(x => x.MovementPointsRequired)
                .First();

            return bestWalkablePositionCandidate;
        }

        
        #endregion
    }
}

public class CandidateMovementPosition
{
    public Position Position { get; set; }
    public int? MovementPointsRequired { get; set; }
    public int DistanceFromTarget { get; set; }
}