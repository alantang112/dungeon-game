using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.GameTemplates;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Geometry;

namespace DungeonGame.Engine.Models
{
    public class World
    {
        public Position HeroPosition { get; set; }
        public HashSet<Position>? Walls { get; set; }
        public List<MonsterPosition>? Monsters { get; set; }

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
            var template = LevelTemplates.Levels.Single(x => x.LevelNumber == levelNumber);
            HeroPosition = template.HeroPosition;

            foreach(var wallPosition in template.WallPositions)
            {
                Walls.Add(wallPosition);
            }

            Monsters = new List<MonsterPosition>();
            foreach(var monster in template.MonsterPositions)
            {
                var monsterType = monster.Item1;
                var monsterPosition = monster.Item2;
                
                Monsters.Add(new MonsterPosition() {
                    Monster = MonsterSpawner.Spawn(monsterType), 
                    Position = monsterPosition
                });
            }
        }
    }
}
