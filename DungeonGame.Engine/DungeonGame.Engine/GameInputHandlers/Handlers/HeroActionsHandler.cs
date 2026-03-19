using System;
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
            InputEventType.HeroActionConfirm
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

                if (gameState.Hero.ActionPoints[SkillType.Movement] < movementPointsRequired)
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
                gameState.Hero.ActionPoints[SkillType.Movement] -= movementPointsRequired;

                return gameState;
            } 
            else if (inputEvent.EventType == InputEventType.HeroActionAttack)
            {
                
            }
            else if (inputEvent.EventType == InputEventType.HeroActionReset)
            {
                
            }
            else if (inputEvent.EventType == InputEventType.HeroActionConfirm)
            {
                
            }

            throw new NotImplementedException();
        }
    }
}
