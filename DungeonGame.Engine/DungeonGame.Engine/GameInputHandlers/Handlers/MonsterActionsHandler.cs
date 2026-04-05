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
                    gameState.ViewData.MonstersAttacking.Add(monsterPosition.Monster.Id); // TODO: add unit test
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

                var wallsAndHero = new List<Position>(gameState.World.Walls)
                {
                    gameState.World.HeroPosition
                };

                var monsterWalkMap = GeometryUtility.PlotValuesByFloodSearch(
                    monsterOriginalPosition,
                    wallsAndHero,
                    FloodSearchHelpers.WalkValueFunction,
                    FloodSearchHelpers.FloodUntilAllSquaresWalked,
                    FloodSearchHelpers.ReturnAllPositions
                );

                var filteredMonsterWalkMap = monsterWalkMap.Where(x => 
                    !gameState.World.Monsters
                        .Where(mp => mp.Position != monsterOriginalPosition)
                        .Any(mp => mp.Position == x.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                if (!filteredMonsterWalkMap.Any())
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }

                var positionCandidates = new List<CandidateMovementPosition>();
                foreach(var position in filteredMonsterWalkMap)
                {
                    var candidate = new CandidateMovementPosition()
                    {
                        Position = position.Key,
                        MovementPointsRequired = position.Value,
                        DistanceFromTarget = walkDistanceFromHeroMap[position.Key],
                        HasLineOfSight = GeometryUtility.HasLineOfSightOf(position.Key, gameState.World.HeroPosition, wallsAndMonstersExcludingSelf),
                        RangeFromTarget = int.MaxValue
                    };

                    if (candidate.HasLineOfSight)
                    {
                        candidate.RangeFromTarget = GeometryUtility.CalculateDistanceBetween(candidate.Position, gameState.World.HeroPosition);

                    }
                        
                    positionCandidates.Add(candidate);
                }

                var monsterAttackRange = monsterPosition.Monster.GetStat(SkillType.AttackRange);

                var idealPosition = positionCandidates
                    // first try to be in line of sight and attack range
                    .OrderBy(x => (x.HasLineOfSight && x.RangeFromTarget <= monsterAttackRange) ? 0 : 1)
                    .ThenByDescending(x => x.HasLineOfSight ? x.RangeFromTarget : int.MaxValue) // then try to be at max range (within line of sight)
                    .ThenBy(x => x.DistanceFromTarget) // then try to be as close as possible to hero
                    .ThenBy(x => x.MovementPointsRequired) // then try to use as less movement as possible
                    .ThenBy(x => x.Position.X) // then try to be in leftmost position
                    .ThenBy(x => x.Position.Y) // then try to be in bottommost position
                    .First();

                if (idealPosition.Position == monsterOriginalPosition)
                {
                    gameState.AddGameMessage(monsterDoesNotMoveMessage);
                    continue;
                }

                var monsterMovement = monsterPosition.Monster.GetStat(SkillType.Movement);
                var walkablePositions = filteredMonsterWalkMap.Where(x =>
                    x.Value <= monsterMovement)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                var idealPositionInMovementRange = walkablePositions.Any(x => x.Key == idealPosition.Position) 
                    ? idealPosition.Position
                    : GetBestWalkableCandidateFromOptimalPositions(positionCandidates, walkablePositions, monsterPosition, gameState, new List<Position>{ idealPosition.Position }).Position;

                if (idealPositionInMovementRange != monsterOriginalPosition)
                {
                    monsterPosition.LastMovementPath = GeometryUtility.FindWalkPath(monsterWalkMap, monsterPosition.Position, idealPositionInMovementRange);
                    monsterPosition.Position = idealPositionInMovementRange;
                    gameState.AddGameMessage(string.Format(GameMessages.MonsterMoves, monsterPosition.Monster.Type, monsterPosition.Position.X, monsterPosition.Position.Y, monsterPosition.Monster.Name));
                    continue;
                }
            }

            return gameState;
        }

        private static List<Position> GetWalkBlockers(GameState gameState)
        {
            var walkBlockers = new List<Position>();
            walkBlockers.AddRange(gameState.World.Walls);
            walkBlockers.Add(gameState.World.HeroPosition);

            return walkBlockers;
        }

        private static List<Position> GetWallsAndMonstersExcludingSelf(MonsterPosition monsterPosition, GameState gameState)
        {
            var result = new List<Position>();
            result.AddRange(gameState.World.Walls);
            result.AddRange(gameState.World.Monsters.Where(mp => mp.Position != monsterPosition.Position).Select(mp => mp.Position));

            return result;
        }

        private static CandidateMovementPosition GetBestWalkableCandidateFromOptimalPositions(List<CandidateMovementPosition> positionCandidates, Dictionary<Position, int> walkablePositions, MonsterPosition monsterPosition, GameState gameState, List<Position> optimalPositions)
        {
            var walkablePositionCandidates = new List<CandidateMovementPosition>();
            walkablePositionCandidates.AddRange(walkablePositions.Select(x =>
                {
                    var result = new CandidateMovementPosition()
                    {
                        Position = x.Key,
                        MovementPointsRequired = x.Value,
                        DistanceFromTarget = int.MaxValue,
                    };

                    var positionCandidate = positionCandidates.First(c => c.Position == x.Key);

                    result.HasLineOfSight = positionCandidate.HasLineOfSight;
                    result.RangeFromTarget = positionCandidate.RangeFromTarget;

                    return result;
                }));

            var walkBlockers = GetWalkBlockers(gameState);

            Func<Position, int, Dictionary<Position, int>, bool> returnPositionsFilter = (Position position, int value, Dictionary<Position, int> floodValues) =>
            {
                return walkablePositionCandidates.Any(x => x.Position == position);
            };

            foreach(var optimalPosition in optimalPositions)
            {
                var distanceToOptimalPositions = GeometryUtility.PlotValuesByFloodSearch(optimalPosition, walkBlockers, FloodSearchHelpers.WalkValueFunction, FloodSearchHelpers.FloodUntilAllSquaresWalked, returnPositionsFilter);

                foreach(var distanceToOptimalPosition in distanceToOptimalPositions)
                {
                    var walkablePositionCandidate = walkablePositionCandidates.Single(x => x.Position == distanceToOptimalPosition.Key);

                    if (distanceToOptimalPosition.Value < walkablePositionCandidate.DistanceFromTarget)
                    {
                        walkablePositionCandidate.DistanceFromTarget = distanceToOptimalPosition.Value;
                    }
                }
            }

            var monsterAttackRange = monsterPosition.Monster.GetStat(SkillType.AttackRange);
            var bestWalkablePositionCandidate = walkablePositionCandidates
                .OrderBy(x => (x.HasLineOfSight && x.RangeFromTarget <= monsterAttackRange) ? 0 : 1)
                .ThenBy(x => x.DistanceFromTarget)
                .ThenBy(x => x.MovementPointsRequired)
                .ThenBy(x => x.Position.X)
                .ThenBy(x => x.Position.Y)
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
    public bool HasLineOfSight { get; set; }
    public int RangeFromTarget { get; set; }
}
