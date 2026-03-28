using System.Collections.Generic;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models.Entities
{
    public class MonsterPosition
    {
        public Monster Monster { get; set; }
        public Position Position { get; set; }
        public List<Position> LastMovementPath { get; set; } = new List<Position>();
    }
}
