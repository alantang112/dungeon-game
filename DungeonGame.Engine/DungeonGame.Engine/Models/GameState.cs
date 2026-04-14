using System.Collections.Generic;
using System.Linq;
using DungeonGame.Engine.GameContent;
using DungeonGame.Engine.Models.Entities;
using DungeonGame.Engine.Models.Enums;
using DungeonGame.Engine.Models.InputEventModels;
using DungeonGame.Engine.Utilities;

namespace DungeonGame.Engine.Models
{
    public class GameState
    {
        public GamePhase GamePhase { get; set; } = GamePhase.Start;
        public EnergyDice EnergyDice { get; set; } = new EnergyDice();
        public Hero Hero { get; set; } = new Hero();
        public World World { get; set; } = new World();
        public int? LevelNumber { get; set; }
        public string? GameMessage { get; set; }
        public List<InputEvent> ScheduledEvents { get; set;} = new List<InputEvent>();
        public List<string> GameMessageLog { get; set; } = new List<string>();
        public ViewData ViewData { get; set; } = new ViewData();
        public int LevelRetriesAvailable { get; set; }

        // snapshots
        public TurnSnapshot? TurnSnapshot { get; set; }
        public LevelSnapshot? LevelSnapshot { get; set; }

        public void AddGameMessage(string message)
        {
            GameMessage = message;
            GameMessageLog.Add(message);

            if (GameMessageLog.Count > GameConstants.GameMessageLogLimit)
                GameMessageLog.RemoveAt(0);
        }

        public void ClearGameMessage()
        {
            GameMessage = null;
        }

        public void LoadTurnSnapshot()
        {
            Hero = TurnSnapshot!.Hero.DeepClone();
            EnergyDice = TurnSnapshot!.EnergyDice.DeepClone();
            World = TurnSnapshot!.World.DeepClone();
        }

        public void SaveTurnSnapshot()
        {
            TurnSnapshot = new TurnSnapshot()
            {
                Hero = Hero.DeepClone(),
                EnergyDice = EnergyDice.DeepClone(),
                World = World.DeepClone(),
            };
        }

        public void LoadLevelSnapshot()
        {
            Hero = LevelSnapshot!.Hero.DeepClone();
            World = LevelSnapshot!.World.DeepClone();
        }

        public void SaveLevelSnapshot()
        {
            LevelSnapshot = new LevelSnapshot()
            {
                Hero = Hero.DeepClone(),
                World = World.DeepClone(),
            };
        }

        // initialize level
        public void InitializeLevel(int levelNumber, bool initRandomWalls = true)
        {
            // init border
            World.InitializeWallBorder();

            // init level
            var template = LevelTemplates.Levels.Single(x => x.LevelNumber == levelNumber);
            World.HeroPosition = template.HeroPosition;

            World.Walls.UnionWith(template.WallPositions);

            // init monsters
            World.Monsters = new List<MonsterPosition>();
            foreach(var monster in template.MonsterPositions)
            {
                var monsterType = monster.Item1;
                var monsterPosition = monster.Item2;
                
                World.Monsters.Add(new MonsterPosition() {
                    Monster = MonsterSpawner.Spawn(monsterType), 
                    Position = monsterPosition
                });
            }

            // init random walls
            if (initRandomWalls)
            {
                var numberOfRandomWalls = RandomUtility.RandomInt(template.RandomWallsCountMin, template.RandomWallsCountMax);
                World.InitializeRandomWalls(numberOfRandomWalls, template.EnforceWallIslands);
            }

            World.RerollsAvailable = 1;
            World.UpgradePointsAvailable = template.UpgradePoints;

            SaveLevelSnapshot();
        }

        public void PostInitializeLevel()
        {
            World.HeroActionPoints.Clear();
            EnergyDice.ResetAssignment();

            GamePhase = GamePhase.UpgradeHero;

            ScheduledEvents.Add(new InputEvent()
            {
                EventType = InputEventType.UpgradeHeroSetup
            });
        }
    }
}
