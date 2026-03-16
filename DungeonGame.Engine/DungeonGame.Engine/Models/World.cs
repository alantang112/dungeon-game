using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class World
    {
        public Position HeroPosition { get; set; }
        public HashSet<Position>? Walls { get; set; }
        public List<(Monster, Position)>? Monsters { get; set; }

        public void InitializeLevel(int levelNumber)
        {
            // init border
            Walls = new HashSet<Position>();

            for (int i = 0; i < GameConstants.LevelSize + 2; i++)
            {
                Walls.Add(new Position(i, 0));
                Walls.Add(new Position(0, i));
                Walls.Add(new Position(GameConstants.LevelSize + 1, i));
                Walls.Add(new Position(i, GameConstants.LevelSize + 1));
            }

            // init level
            var template = LevelDefinitions.Levels.Single(x => x.LevelNumber == levelNumber);
            HeroPosition = template.HeroPosition;

            foreach(var wallPosition in template.WallPositions)
            {
                Walls.Add(wallPosition);
            }

            Monsters = new List<(Monster, Position)>();
            foreach(var monster in template.MonsterPositions)
            {
                var monsterType = monster.Item1;
                var monsterPosition = monster.Item2;
                
                Monsters.Add((MonsterSpawner.Spawn(monsterType), monsterPosition));
            }
        }
    }
}
