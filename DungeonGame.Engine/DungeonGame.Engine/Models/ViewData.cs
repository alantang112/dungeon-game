using System.Collections.Generic;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public class ViewData
    {
        public List<Position> HeroCanWalkPositions { get; set; } = new List<Position>();
        public List<Position> HeroCanAttackPositions { get; set; } = new List<Position>();
    }
}
