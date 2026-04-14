using System;
using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.Geometry;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.GameInputHandlers.Handlers
{
    internal class HeroActionsHandler : AbstactGameInputHandler
    {
        protected override GamePhase HandledGamePhase => GamePhase.HeroActions;

        protected override InputEventType[] HandledInputEventTypes => new InputEventType[] {
            InputEventType.HeroActionMove, 
            InputEventType.HeroActionAttack, 
            InputEventType.HeroActionReset,
            InputEventType.HeroActionEnd
        };

        public static bool HeroCanWalkTo(World world, Position newPosition, string? heroName = null)
        {
            var movementPointsRequired = GeometryUtility.CalculateDistanceBetween(world.HeroPosition, newPosition);

            if (movementPointsRequired > GameConstants.MovementPointsDiagonal || movementPointsRequired == 0)
            {
                return false;
            }

            if (movementPointsRequired == GameConstants.MovementPointsDiagonal)
            {
                var blockers = new List<Position>();
                blockers.AddRange(world.Walls);

                if (blockers.Contains(new Position(world.HeroPosition.X, newPosition.Y))
                    && blockers.Contains(new Position(newPosition.X, world.HeroPosition.Y)))
                {
                    return false;
                }
            }

            if (world.HeroActionPoints[SkillType.Movement] < movementPointsRequired)
            {
                return false;
            }

            if (world.Walls.Contains(newPosition) || world.Monsters.Any(x => x.Position == newPosition))
            {
                return false;
            }

            return true;
        }

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.HeroActionMove)
            {
                var parameters = (HeroActionMoveEventParameters) inputEvent.EventParameters!;
                
                var newPosition = new Position(parameters.X, parameters.Y);

                var canWalk = HeroCanWalkTo(gameState.World, newPosition, gameState.Hero.Name);

                if (!canWalk)
                    return gameState;
                
                gameState.World.HeroActionPoints[SkillType.Movement] -= GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, newPosition);
                gameState.World.HeroPosition = newPosition;

                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.HeroActionAttack)
            {
                var parameters = (HeroActionAttackEventParameters) inputEvent.EventParameters!;
                var attackAtPosition = new Position(parameters.X, parameters.Y);
                
                var monsterPosition = gameState.World.Monsters.FirstOrDefault(x => x.Position == attackAtPosition);

                if (monsterPosition == null)
                {
                    return gameState;
                }

                if (GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, monsterPosition.Position) > gameState.Hero.Stats[SkillType.AttackRange])
                {
                    return gameState;
                }

                var blockers = new List<Position>();
                blockers.AddRange(gameState.World.Walls);
                blockers.AddRange(gameState.World.Monsters.Select(mp => mp.Position));
                if (!GeometryUtility.HasLineOfSightOf(gameState.World.HeroPosition, monsterPosition.Position, blockers))
                {
                    return gameState;
                }

                var monsterDefence = monsterPosition.Monster.GetStat(SkillType.Defence);
                if (monsterDefence > gameState.World.HeroActionPoints[SkillType.Attack])
                {
                    return gameState;
                }

                gameState.ViewData.MonsterAttackedByHero = monsterPosition;
                monsterPosition.Monster.Health -= 1;
                MonsterSpecialFunctions.PostDamageFunction(monsterPosition.Monster, gameState);

                gameState.World.HeroActionPoints[SkillType.Attack] -= monsterDefence;

                if (monsterPosition.Monster.Health <= 0)
                {
                    gameState.World.Monsters.Remove(monsterPosition);
                    
                    if (gameState.LevelNumber == GameConstants.NightmareLevelNumber)
                    {
                        if (!gameState.World.Monsters.Any(x => x.Monster.Type == MonsterType.Nightmare))
                        {
                            gameState.GamePhase = GamePhase.GameEnd;
                        }
                    }
                    else
                    {
                        if (!gameState.World.Monsters.Any())
                        {
                            gameState.GamePhase = GamePhase.LevelEnd;
                        }
                    }
                }
                
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.HeroActionReset)
            {
                gameState.LoadTurnSnapshot();
                gameState.GamePhase = GamePhase.EnergyDiceAssignment;

                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.HeroActionEnd)
            {
                // Trigger monster actions
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                   EventType = InputEventType.MonstersMove
                });
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                   EventType = InputEventType.MonstersAttack
                });

                gameState.GamePhase = GamePhase.MonsterActions;
                return gameState;
            }

            throw new NotImplementedException();
        }
    }
}
