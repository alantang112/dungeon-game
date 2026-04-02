import { useGameEngine } from './hooks/useGameEngine';
import { MonsterPosition, Position, GameState, type SkillType, EnergyDice } from './models/GameEngineModels'
import { Tile } from './props/Tile';
import { GameLog } from './props/GameLog';
import { CharacterStats } from './props/CharacterStats';
import GameActions from './interfaces/gameEngineInterface';
import { HeroMaxHealth, LevelSize, statColor, statText, getMonsterPathColor, DebugMode, howToPlayUrl, levelNameColor } from './constants/gameConstants';
import './App.css'
import type { AvailableButton } from './models/AvailableButton';
import { ButtonsRow } from './props/ButtonsRow';
import type { Arrow } from './models/Arrow';
import { Dice } from './props/Dice';
import { ShufflingDice } from './props/ShufflingDice';
import type { ReactNode } from "react";
import type { TileData } from './models/TileData';

function App() {
  const { state, dispatch, isReady } = useGameEngine();

  if (!isReady) return <div>Loading game engine...</div>;

  const handleTileClick = async (state: GameState, x: number, y: number) => {
    if (state.GamePhase != "HeroActions")
      return;

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
      const tileData = getTileData(state, x, y);

      gridRows.push(
        <div key={`${x}-${y}`} onClick={() => handleTileClick(state, x, y)}>
          <Tile
          x={x}
          y={y}
          tileType={tileData.tileType}
          name={tileData.name ?? ''}
          heroCanWalk={state.ViewData?.HeroCanWalkPositions?.some(p => p.X == x && p.Y == y) ?? false}
          heroCanAttack={state.ViewData?.HeroCanAttackPositions?.some(p => p.X == x && p.Y == y) ?? false}
          health={tileData.health}
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

  var currentDiceToAssignIndex: number = 0;

  if (heroInitialized) {
    if (state.GamePhase == "EnergyDicePreRoll" || state.GamePhase == "EnergyDiceAssignment")
    {
      heroEnergy = {
        "Movement": state.Hero!.Stats!["Movement"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Movement"),
        "Attack": state.Hero!.Stats!["Attack"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Attack"),
        "Defence": state.Hero!.Stats!["Defence"] + GetAssignedEnergyDiceValue(state.EnergyDice!, "Defence"),
        "AttackRange": 0
      };

      currentDiceToAssignIndex = state.EnergyDice!.AssignedSkills!.findIndex(x => x == null);
    }
    else if (state.World?.HeroActionPoints)
    {
      heroEnergy = { ...state.World!.HeroActionPoints! };
    }
  }

  const monsterRows: ReactNode[] = [];
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

    // only display monster movement path during MonsterActions phase
    if (state.GamePhase == "MonsterActions")
    {
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
  }

  return (
    <div className="bg-slate-950">
      <div className="flex flex-col items-center flex-start gap-5 sm:justify-between h-screen bg-slate-900 p-4 sm:max-w-400 mx-auto">
        {/* Character stats */}
        <div className="sm:max-h-2/10 sm:grid sm:p-5 font-mono grid sm:grid-cols-1 lg:grid-cols-12 w-full">
          {/* Hero stats */}
          <div 
            className="col-span-1 lg:w-auto lg:col-span-3 flex flex-col gap-2"
            onClick={() => { if (DebugMode) { navigator.clipboard.writeText(JSON.stringify(state)); console.log('gameState copied to clipboard') } return; }}
            >
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
          {/* Monster stats */}
          <div className="hidden sm:grid col-span-1 lg:w-auto lg:col-span-3 lg:col-start-10 flex flex-col gap-2">
            {monsterRows}
          </div>
        </div>
        {/* Game grid */}
        <div className="relative inline-block border-2 border-slate-600 bg-slate-700 p-1">
          <div className={`${levelNameColor(state.LevelNumber ?? 1)} p-1`}>{state.LevelNumber && `Level ${state.LevelNumber}`}</div>
          <div className="relative">
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
              style={{ zIndex: 20 }}
            >
              <defs>
                <marker id="arrowhead" markerWidth="4" markerHeight="4" refX="2" refY="2" orient="auto" markerUnits="strokeWidth">
                  <polygon points="0 0, 4 2, 0 4" fill="context-stroke" fillOpacity="0.3" />
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
                  strokeOpacity="0.3"
                  markerEnd={path.hasArrowHead ? "url(#arrowhead)" : ""}
                  pathLength="1"
                  strokeDasharray={`${path.hasArrowHead ? "0.95" : "1"} 1`}
                />
              );
              } )}
            </svg>
          </div>
        </div>
        {/* Dice and Buttons */}
        <div className="flex flex-col gap-8 justify-around sm:h-50">
          <div className="flex gap-3 items-center justify-center">
            {
              state.GamePhase == "EnergyDicePreRoll" 
                && <>
                    <ShufflingDice />
                    <ShufflingDice />
                    <ShufflingDice />
                  </>
            }
            {
              state.GamePhase == "EnergyDiceAssignment" 
                && <>
                    {state.EnergyDice!.Dice!.map((diceNumber, index) => {
                          return (
                            <div key={`dice-to-assign-${index}`}>
                              <Dice number={diceNumber} active={currentDiceToAssignIndex == index} disabled={currentDiceToAssignIndex > index} />
                            </div>
                          )
                        })}
                    </>
            }
          </div>
          <div className="flex gap-6 items-center justify-center">
              <ButtonsRow
                buttons={GetAvailableButtons(state)}
                eventDispatcher={dispatch}
              />
          </div>
          <div className="flex gap-6 mb-4 items-center justify-center">
            <ButtonsRow
              buttons={GetAvailableButtonsRow2(state)}
              eventDispatcher={dispatch}
              />
          </div>
        </div>  
        {/* Game Log */}
        <div className="hidden sm:block w-full">
            <GameLog messages={state.GameMessageLog ?? []}/>
        </div>
      </div>
    </div>
  )
}

const getTileData = (state: GameState, x: number, y: number) : TileData => {
    if (state?.World?.HeroPosition?.X === x && state?.World?.HeroPosition?.Y === y) return { tileType: "Hero", health: state!.Hero!.Health, name: state!.Hero!.Name  };
    if (state?.World?.Walls?.some((w: Position) => w.X === x && w.Y === y)) return { tileType: "Wall" };
    const monster = state?.World?.Monsters?.find((m: MonsterPosition) => m.Position.X === x && m.Position.Y === y);
    if (monster) return { tileType: monster.Monster.Type, health: monster.Monster.Health, name: monster.Monster.Name };
    return { tileType: "Empty" };
};

const GetAvailableButtons = (state: GameState) : AvailableButton[] => {
  const availableButtons: AvailableButton[] = [];

  if (state.GamePhase == "Start")
  {
    availableButtons.push({
      textNode: <>New game</>,
      gameEventOnClick: GameActions.NewGameEvent()
    });
  } 
  else if (state.GamePhase == "EnergyDicePreRoll")
  {
    availableButtons.push({
      textNode: <>Roll energy dice</>,
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
            textNode: <>⚡{currentDiceValue} ⟶ <span className={`${statColor[skillType]}`}>{`${statText[skillType]} `}</span></>,
            gameEventOnClick: GameActions.AssignDiceEvent(currentDiceIndex, skillType)
          });
        }
        else {
          availableButtons.push({
            textNode: <>⚡{GetAssignedEnergyDiceValue(state.EnergyDice, skillType)} ⟶ <span className={`${statColor[skillType]}`}>{`${statText[skillType]} `}</span></>,
            disabled: true
          });
        }
      });
    }
  } 
  else if (state.GamePhase == "HeroActions")
  {
    availableButtons.push({
      textNode: <>Reset turn</>,
      gameEventOnClick: GameActions.HeroActionResetEvent()
    });

    availableButtons.push({
      textNode: <>End turn</>,
      gameEventOnClick: GameActions.HeroActionEndEvent()
    });
  }
  else if (state.GamePhase == "MonsterActions")
  {
    availableButtons.push({
      textNode: <>Continue</>,
      gameEventOnClick: GameActions.MonsterActionEndEvent()
    });
  }
  else if (state.GamePhase == "LevelEnd")
  {
    availableButtons.push({
      textNode: <>+1 <span className={`${statColor["Movement"]}`}>{statText["Movement"]}</span></>,
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Movement")
    });
    availableButtons.push({
      textNode: <>+1 <span className={`${statColor["Attack"]}`}>{statText["Attack"]}</span></>,
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Attack")
    });
    availableButtons.push({
      textNode: <>+1 <span className={`${statColor["Defence"]}`}>{statText["Defence"]}</span></>,
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Defence")
    });
  }

  return availableButtons;
}

const GetAvailableButtonsRow2 = (state: GameState) : AvailableButton[] => {
  const availableButtons: AvailableButton[] = [];

  if (state.GamePhase == "Start")
  {
    availableButtons.push({
      textNode: <>How to play</>,
      onClick: () => { 
          const newTab = window.open(howToPlayUrl, '_blank');
          if (newTab)
            newTab.focus();
          else {
            alert('Google "one card dungeon rulebook"');
          }
       }
    });
  } 
  else if (state.GamePhase == "EnergyDiceAssignment")
  {
    availableButtons.push({
      textNode: <>Reset dice assignment</>,
      gameEventOnClick: GameActions.ResetDiceEvent(),
      smaller: true
    });

    availableButtons.push({
      textNode: <>Reroll (once per level)</>,
      gameEventOnClick: GameActions.RerollDiceEvent(),
      disabled: (state.World?.RerollsAvailable ?? 0) <= 0,
      smaller: true
    })
  } 
  else if (state.GamePhase == "LevelEnd")
  {
    availableButtons.push({
      textNode: <>+1 <span className={`${statColor["AttackRange"]}`}>{statText["AttackRange"]}</span></>,
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("AttackRange")
    });
    availableButtons.push({
      textNode: <><span className='text-green-300'>Replenish Health</span></>,
      gameEventOnClick: GameActions.NextLevelReplenishHealthEvent()
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

export default App
