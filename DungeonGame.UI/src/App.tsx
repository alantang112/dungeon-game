import { useGameEngine } from './hooks/useGameEngine';
import { MonsterPosition, Position, GameState, type SkillType, EnergyDice } from './models/GameEngineModels'
import { Tile } from './props/Tile';
import { GameLog } from './props/GameLog';
import { CharacterStats } from './props/CharacterStats';
import GameActions from './interfaces/gameEngineInterface';
import { HeroMaxHealth, LevelSize } from './constants/gameConstants';
import './App.css'
import type { AvailableButton } from './models/AvailableButton';
import { ButtonsRow } from './props/ButtonsRow';
import type { Arrow } from './models/Arrow';

function App() {
  const { state, dispatch, isReady } = useGameEngine();

  if (!isReady) return <div>Loading game engine...</div>;

  const handleTileClick = async (state: GameState, x: number, y: number) => {
    try {
      if (state.World?.Monsters?.some(mp => mp.Position.X == x && mp.Position.Y == y))
      {
        await dispatch(GameActions.HeroAttackEvent(x, y));
      }
      else {
        await dispatch(GameActions.HeroMoveEvent(x, y));
      }
    } catch (e) {
      alert("Could not process input event!");
    }
  }

  const gridSize : number = LevelSize;
  const gridRows = [];

  for (let y = gridSize; y >= 1; y--) {
    for (let x = 1; x <= gridSize; x++) {
      gridRows.push(
        <div key={`${x}-${y}`} onClick={() => handleTileClick(state, x, y)}>
          <Tile
          x={x}
          y={y}
          backgroundColor={`${getTileBackgroundColor(state, x, y)}`}
          />
        </div>
      );
    }
  }

  const heroInitialized: boolean = state.Hero?.Name != undefined;

  var heroEnergy: Record<SkillType, number> = {
    "Movement": 0,
    "Attack": 0,
    "Defence": 0,
    "AttackRange": 0
  };

  if (heroInitialized) {
    if (state.GamePhase == "EnergyDiceAssignment")
    {
      heroEnergy = {
        "Movement": state.Hero!.Stats!["Movement"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Movement"),
        "Attack": state.Hero!.Stats!["Attack"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Attack"),
        "Defence": state.Hero!.Stats!["Defence"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Defence"),
        "AttackRange": 0
      }
    }
    else if (state.World?.HeroActionPoints)
    {
      heroEnergy = { ...state.World!.HeroActionPoints! };
    }
  }

  const monsterRows = [];
  const monsterPaths: Arrow[] = [];
  const monsterCount: number = state.World?.Monsters?.length ?? 0;

  for (let monsterIndex = 0; monsterIndex < monsterCount; monsterIndex++)
  {
    const monster = state.World!.Monsters![monsterIndex].Monster;
    monsterRows.push(
      <div key={`monster-${monsterIndex}`}>
        <CharacterStats 
              name={`${monster.Type} ${monster.Name}`}
              health={monster.Health}
              maxHealth={monster.MaxHealth}
              stats={monster.Stats}
              energy={undefined} 
              displayEnergy={false}
              isEnemy={true}
            />
      </div>
    )

    const lastMovementPath = state.World!.Monsters![monsterIndex].LastMovementPath;

    if (lastMovementPath)
    {
      const lastMovementPathLength: number = lastMovementPath.length;
      for (let j = 1; j < lastMovementPathLength; j++)
      {
        monsterPaths.push({
          startX: lastMovementPath[j - 1].X,
          startY: lastMovementPath[j - 1].Y,
          endX: lastMovementPath[j].X,
          endY: lastMovementPath[j].Y,
          setNumber: monsterIndex,
          hasArrowHead: j == (lastMovementPath.length - 1)
        })
      }
    }
  }

  return (
    <>
      <div className="absolute top-0 left-0 w-full h-[350px] p-5 font-mono grid grid-cols-2 gap-5 lg:grid-cols-12">
        {/* LEFT SIDE: INPUT */}
        <div className="col-span-1 lg:w-auto lg:col-span-3 flex flex-col gap-2">
          <CharacterStats 
            name={heroInitialized ? state.Hero!.Name! : "Hero"}
            health={heroInitialized ? state.Hero!.Health! : HeroMaxHealth}
            maxHealth={HeroMaxHealth}
            stats={heroInitialized ? state.Hero!.Stats : undefined}
            energy={heroEnergy} 
            displayEnergy={true}
            isEnemy={false}
          />
        </div>
        {/* RIGHT SIDE: OUTPUT */}
        <div className="col-span-1 lg:w-auto lg:col-span-3 lg:col-start-10 flex flex-col gap-2">
          {monsterRows}
        </div>
      </div>

      <div className="flex flex-col items-center justify-center min-h-screen bg-slate-900 p-4">
        <div className="relative inline-block border-2 border-slate-600 bg-slate-700 p-1">
          {/* The Grid */}
          <div 
            className="grid gap-1 aspect-square"
            style={{ gridTemplateColumns: `repeat(${gridSize}, minmax(0, 1fr))`, display: 'grid' }}
          >
            {gridRows}
          </div>

          {/* The Overlay Layer */}
          <svg 
            className="absolute inset-0 pointer-events-none" 
            viewBox={`0 0 ${gridSize} ${gridSize}`}
            preserveAspectRatio="none"
          >
            <defs>
              <marker id="arrowhead" markerWidth="4" markerHeight="4" refX="2" refY="2" orient="auto" markerUnits="strokeWidth">
                <polygon points="0 0, 4 2, 0 4" fill="context-stroke" fill-opacity="0.5" />
              </marker>
            </defs>

            {monsterPaths.map((path, i) => {
              return (
              <line
                key={i}
                x1={path.startX - 0.5} 
                y1={gridSize - path.startY + 0.5}
                x2={path.endX - 0.5}
                y2={gridSize - path.endY + 0.5}
                stroke={getMonsterPathColor(path.setNumber)}
                strokeWidth="0.05"
                strokeOpacity="0.5"
                //strokeLinecap="round"
                markerEnd={path.hasArrowHead ? "url(#arrowhead)" : ""}

                pathLength="1"
                strokeDasharray={`${path.hasArrowHead ? "0.9" : "1"} 1`}
              />
            );
            } )}
          </svg>
        </div>
        
        <ButtonsRow
          buttons={GetAvailableButtons(state)}
          eventDispatcher={dispatch}
          />
        <ButtonsRow
          buttons={GetAvailableButtonsRow2(state)}
          eventDispatcher={dispatch}
          />
      </div>
      <GameLog messages={state.GameMessageLog ?? []}/>
    </>
  )
}

const getTileBackgroundColor = (state: GameState, x: number, y: number) : string => {
    if (state?.World?.HeroPosition?.X === x && state?.World?.HeroPosition?.Y === y) return "green";
    if (state?.World?.Walls?.some((w: Position) => w.X === x && w.Y === y)) return "darkgrey";
    if (state?.World?.Monsters?.some((m: MonsterPosition) => m.Position.X === x && m.Position.Y === y)) return "darkred";
    return "";
};

const GetAvailableButtons = (state: GameState) : AvailableButton[] => {
  const availableButtons: AvailableButton[] = [];

  if (state.GamePhase == "Start")
  {
    availableButtons.push({
      text: "New Game",
      gameEventOnClick: GameActions.NewGameEvent()
    });
  } 
  else if (state.GamePhase == "EnergyDicePreRoll")
  {
    availableButtons.push({
      text: "Roll Energy Dice",
      gameEventOnClick: GameActions.RollDiceEvent()
    });
  } 
  else if (state.GamePhase == "EnergyDiceAssignment")
  {
    const assignableSkillTypes: SkillType[] = ["Movement", "Attack", "Defence"];
    const currentDiceIndex: number = state.EnergyDice?.AssignedSkills?.findIndex(x => x == null) ?? -1;

    if (currentDiceIndex >= 0)
    {
      const currentDiceValue: number = state.EnergyDice!.Dice![currentDiceIndex];

      assignableSkillTypes.forEach(skillType => {
        if (!state.EnergyDice?.AssignedSkills?.includes(skillType))
        {
          availableButtons.push({
            text: `Assign +${currentDiceValue}⚡ to ${skillType}`,
            gameEventOnClick: GameActions.AssignDiceEvent(currentDiceIndex, skillType)
          });
        }
      });
    }
  } 
  else if (state.GamePhase == "MonsterActions")
  {
    availableButtons.push({
      text: "Continue",
      gameEventOnClick: GameActions.MonsterActionEndEvent()
    });
  }
  else if (state.GamePhase == "LevelEnd")
  {
    availableButtons.push({
      text: "+1 Movement",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Movement")
    });
    availableButtons.push({
      text: "+1 Attack",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Attack")
    });
    availableButtons.push({
      text: "+1 Defence",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Defence")
    });
    availableButtons.push({
      text: "+1 Range",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("AttackRange")
    });
    availableButtons.push({
      text: "Replenish Health",
      gameEventOnClick: GameActions.NextLevelReplenishHealthEvent()
    });
  }

  return availableButtons;
}

const GetAvailableButtonsRow2 = (state: GameState) : AvailableButton[] => {
  const availableButtons: AvailableButton[] = [];

  if (state.GamePhase == "EnergyDiceAssignment")
  {
    availableButtons.push({
      text: "Reset Dice Assignment",
      gameEventOnClick: GameActions.ResetDiceEvent()
    });
  } 
  else if (state.GamePhase == "HeroActions")
  {
    availableButtons.push({
      text: "Reset Turn",
      gameEventOnClick: GameActions.HeroActionResetEvent()
    });

    availableButtons.push({
      text: "End Turn",
      gameEventOnClick: GameActions.HeroActionEndEvent()
    });
  }

  return availableButtons;
}

const GetAssignedEnergyDiceValue = (energyDice: EnergyDice, skill: SkillType): number => {
  if (energyDice?.AssignedSkills?.includes(skill))
  {
    const index = energyDice!.AssignedSkills!.findIndex(x => x == skill);
    return energyDice!.Dice![index];
  }

  return 0;
}

const monsterPathColors: string[] = [
  "#efc93d",
  "#ef7e3d",
  "#8cdd45",
  "#44b9d3",
  "#e24fe5",
];

const getMonsterPathColor = (index: number) => monsterPathColors[index];

export default App
