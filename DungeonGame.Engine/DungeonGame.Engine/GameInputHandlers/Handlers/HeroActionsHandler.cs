using System;
using System.Collections.Generic;
using System.Linq;
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

        public override GameState TransformGameState(GameState gameState, InputEvent inputEvent)
        {
            if (inputEvent.EventType == InputEventType.HeroActionMove)
            {
                var parameters = (HeroActionMoveEventParameters) inputEvent.EventParameters!;
                
                var newPosition = new Position(parameters.X, parameters.Y);
                var movementPointsRequired = GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, newPosition);

                if (movementPointsRequired > 3 || movementPointsRequired == 0)
                {
                    gameState.GameMessage = GameMessages.CanOnlyMoveAdjacently;
                    return gameState;
                }

                if (gameState.World.HeroActionPoints[SkillType.Movement] < movementPointsRequired)
                {
                    gameState.GameMessage = GameMessages.NotEnoughMovementActionPoints;
                    return gameState;
                }

                if (gameState.World.Walls.Contains(newPosition) || gameState.World.Monsters.Any(x => x.Position == newPosition))
                {
                    gameState.GameMessage = GameMessages.CannotMoveToThatSpace;
                    return gameState;
                }

                gameState.World.HeroPosition = newPosition; 
                gameState.World.HeroActionPoints[SkillType.Movement] -= movementPointsRequired;

                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.HeroActionAttack)
            {
                var parameters = (HeroActionAttackEventParameters) inputEvent.EventParameters!;
                var attackAtPosition = new Position(parameters.X, parameters.Y);
                
                var monsterPosition = gameState.World.Monsters.FirstOrDefault(x => x.Position == attackAtPosition);

                if (monsterPosition == null)
                {
                    gameState.GameMessage = GameMessages.NoMonsterToAttackAtThatSpace;
                    return gameState;
                }

                if (GeometryUtility.CalculateDistanceBetween(gameState.World.HeroPosition, monsterPosition.Position) > gameState.Hero.Stats[SkillType.AttackRange])
                {
                    gameState.GameMessage = GameMessages.MonsterNotInRangeToAttack;
                    return gameState;
                }

                var blockers = new List<Position>();
                blockers.AddRange(gameState.World.Walls);
                blockers.AddRange(gameState.World.Monsters.Select(mp => mp.Position));
                if (!GeometryUtility.HasLineOfSightOf(gameState.World.HeroPosition, monsterPosition.Position, blockers.ToArray()))
                {
                    gameState.GameMessage = GameMessages.MonsterNotInLineOfSightToAttack;
                    return gameState;
                }

                if (monsterPosition.Monster.Stats[SkillType.Defence] > gameState.World.HeroActionPoints[SkillType.Attack])
                {
                    gameState.GameMessage = GameMessages.NotEnoughAttackToAttackMonster;
                    return gameState;
                }

                monsterPosition.Monster.Health -= 1;
                gameState.World.HeroActionPoints[SkillType.Attack] -= monsterPosition.Monster.Stats[SkillType.Defence];

                if (monsterPosition.Monster.Health <= 0)
                {
                    // TODO: Add a game message?
                    gameState.World.Monsters.Remove(monsterPosition);

                    if (!gameState.World.Monsters.Any())
                    {
                        gameState.GamePhase = GamePhase.LevelEnd;
                        // TODO: game message?
                    }
                }
                else
                {
                    // TODO: Add a game message?
                }
                
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.HeroActionReset)
            {
                gameState.World = gameState.WorldSnapshot.DeepClone();
                return gameState;
            }
            else if (inputEvent.EventType == InputEventType.HeroActionEnd)
            {
                // Trigger monster actions
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                   EventType =  InputEventType.MonstersMove
                });
                gameState.ScheduledEvents.Add(new InputEvent()
                {
                   EventType =  InputEventType.MonstersAttack
                });

                gameState.GamePhase = GamePhase.MonsterActions;
                return gameState;
            }

            throw new NotImplementedException();
        }
    }
}
