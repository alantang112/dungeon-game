using System;
using System.Collections.Generic;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public class ViewData
    {
        public List<Position> HeroCanWalkPositions { get; set; } = new List<Position>();
        public List<Position> HeroCanAttackPositions { get; set; } = new List<Position>();
        public List<Guid> MonstersAttacking { get; set; } = new List<Guid>();
        public MonsterPosition? MonsterAttackedByHero { get; set; }
    }
}
