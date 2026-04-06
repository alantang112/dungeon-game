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

        public static (bool, string?) HeroCanWalkTo(World world, Position newPosition, string? heroName = null)
        {
            var movementPointsRequired = GeometryUtility.CalculateDistanceBetween(world.HeroPosition, newPosition);

            if (movementPointsRequired > GameConstants.MovementPointsDiagonal || movementPointsRequired == 0)
            {
                return (false, string.Format(GameMessages.CanOnlyMoveAdjacently, heroName));
            }

            if (movementPointsRequired == GameConstants.MovementPointsDiagonal)
            {
                var blockers = new List<Position>();
                blockers.AddRange(world.Walls);

                if (blockers.Contains(new Position(world.HeroPosition.X, newPosition.Y))
                    && blockers.Contains(new Position(newPosition.X, world.HeroPosition.Y)))
                {
                    return (false, string.Format(GameMessages.CannotMoveToThatSpace, heroName));
                }
            }

            if (world.HeroActionPoints[SkillType.Movement] < movementPointsRequired)
            {
                return (false, string.Format(GameMessages.NotEnoughMovementActionPoints, heroName));
            }

            if (world.Walls.Contains(newPosition) || world.Monsters.Any(x => x.Position == newPosition))
            {
                return (false, string.Format(GameMessages.CannotMoveToThatSpace, heroName));
            }

            return (true, null);
        }

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.HeroActionMove)
            {
                var parameters = (HeroActionMoveEventParameters) inputEvent.EventParameters!;
                
                var newPosition = new Position(parameters.X, parameters.Y);

                var (canWalk, gameMessage) = HeroCanWalkTo(gameState.World, newPosition, gameState.Hero.Name);

                if (!string.IsNullOrEmpty(gameMessage))
                    gameState.AddGameMessage(string.Format(GameMessages.CanOnlyMoveAdjacently, gameState.Hero.Name));

                if (!canWalk)
                    return gameState;
                
                gameState.World.HeroActionPoints[SkillType.Movement] -= GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, newPosition);
                gameState.World.HeroPosition = newPosition;
                gameState.AddGameMessage(string.Format(GameMessages.HeroMovedTo, gameState.Hero.Name, newPosition.X, newPosition.Y));

                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.HeroActionAttack)
            {
                var parameters = (HeroActionAttackEventParameters) inputEvent.EventParameters!;
                var attackAtPosition = new Position(parameters.X, parameters.Y);
                
                var monsterPosition = gameState.World.Monsters.FirstOrDefault(x => x.Position == attackAtPosition);

                if (monsterPosition == null)
                {
                    gameState.AddGameMessage(GameMessages.NoMonsterToAttackAtThatSpace);
                    return gameState;
                }

                if (GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, monsterPosition.Position) > gameState.Hero.Stats[SkillType.AttackRange])
                {
                    gameState.AddGameMessage(string.Format(GameMessages.MonsterNotInRangeToAttack, monsterPosition.Monster.Type, monsterPosition.Monster.Name));
                    return gameState;
                }

                var blockers = new List<Position>();
                blockers.AddRange(gameState.World.Walls);
                blockers.AddRange(gameState.World.Monsters.Select(mp => mp.Position));
                if (!GeometryUtility.HasLineOfSightOf(gameState.World.HeroPosition, monsterPosition.Position, blockers))
                {
                    gameState.AddGameMessage(string.Format(GameMessages.MonsterNotInLineOfSightToAttack, monsterPosition.Monster.Type, monsterPosition.Monster.Name));
                    return gameState;
                }

                var monsterDefence = monsterPosition.Monster.GetStat(SkillType.Defence);
                if (monsterDefence > gameState.World.HeroActionPoints[SkillType.Attack])
                {
                    gameState.AddGameMessage(string.Format(GameMessages.NotEnoughAttackToAttackMonster, gameState.Hero.Name, monsterPosition.Monster.Type, monsterPosition.Monster.Name));
                    return gameState;
                }

                gameState.ViewData.MonsterAttackedByHero = monsterPosition;
                monsterPosition.Monster.Health -= 1;
                MonsterSpecialFunctions.PostDamageFunction(monsterPosition.Monster, gameState);

                gameState.World.HeroActionPoints[SkillType.Attack] -= monsterDefence;
                gameState.AddGameMessage(string.Format(GameMessages.HeroAttacksMonster, gameState.Hero.Name, monsterPosition.Monster.Type, 
                    monsterPosition.Monster.Health, monsterPosition.Monster.Name));

                if (monsterPosition.Monster.Health <= 0)
                {
                    gameState.AddGameMessage(string.Format(GameMessages.MonsterDefeated, monsterPosition.Monster.Type, monsterPosition.Monster.Name));
                    gameState.World.Monsters.Remove(monsterPosition);
                    
                    if (!gameState.World.Monsters.Any())
                    {
                        gameState.GamePhase = GamePhase.LevelEnd;
                        gameState.AddGameMessage(GameMessages.AllMonstersDefeated);
                    }
                }
                
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.HeroActionReset)
            {
                gameState.World = gameState.WorldSnapshot.DeepClone();
                gameState.EnergyDice = gameState.EnergyDiceSnapshot.DeepClone();
                gameState.GamePhase = GamePhase.EnergyDiceAssignment;

                gameState.AddGameMessage(string.Format(GameMessages.HeroReset, gameState.Hero.Name, gameState.Hero.isMaleName ? "his" : "her"));
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
