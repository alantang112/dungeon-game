import { useGameEngine } from './hooks/useGameEngine';
import { GameInputEvent, MonsterPosition, Position, GameState, type SkillType, EnergyDice } from './models/GameEngineModels'
import { Tile } from './props/Tile';
import { GameLog } from './props/GameLog';
import { CharacterStats } from './props/CharacterStats';
import GameActions from './interfaces/gameEngineInterface';
import './App.css'

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

  const gridSize : number = 5;
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
  
  const monsterCount = state.World?.Monsters?.length ?? 0;
  for (let i = 0; i < monsterCount; i++)
  {
    const monster = state.World!.Monsters![i].Monster;
    monsterRows.push(
      <div key={`monster-${i}`}>
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
  }

  return (
    <>
      <div className="absolute top-0 left-0 w-full h-[350px] p-5 font-mono grid grid-cols-2 gap-5 lg:grid-cols-12">
        {/* LEFT SIDE: INPUT */}
        <div className="col-span-1 lg:w-auto lg:col-span-3 flex flex-col gap-2">
          <CharacterStats 
            name={heroInitialized ? state.Hero!.Name! : "Hero"}
            health={heroInitialized ? state.Hero!.Health! : 10}
            maxHealth={10}
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
        <div 
          className="grid grid-cols-5 gap-1 aspect-square border-2 border-slate-600 bg-slate-700 p-1"
          style={{ gridTemplateColumns: `repeat(${gridSize}, minmax(0, 1fr))` }}
        >
          {gridRows}
        </div>
        <div className="flex gap-6 mt-15 h-12 items-center justify-center">
          {GetAvailableButtons(state)?.map((button: AvailableButton, index: number) => (
            <button
              key={index}
              onClick={async () => await dispatch(button.gameEventOnClick)}
              className="
          
              inline-block
              px-7 py-3
              bg-indigo-600 hover:bg-indigo-500 
              text-white font-bold tracking-wide
              rounded-full shadow-[0_10px_20px_rgba(0,0,0,0.4)] 
              transform transition-all active:scale-95
              border border-indigo-400/30
            "
            >
              {button.text}
            </button>
          ))}
      </div>
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

interface AvailableButton {
  text: string;
  gameEventOnClick: GameInputEvent;
}

const GetAvailableButtons = (state: GameState) : AvailableButton[] => {
  const availableButtons: AvailableButton[] = [];

  if (state.GamePhase == "Start")
  {
    availableButtons.push({
      text: "New Game",
      gameEventOnClick: GameActions.NewGameEvent("Lil Timmy") // todo: user to enter name
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
            text: `Assign +${currentDiceValue} energy to ${skillType}`,
            gameEventOnClick: GameActions.AssignDiceEvent(currentDiceIndex, skillType)
          });
        }
      });
    }

    availableButtons.push({
      text: "Reset Dice Assignment",
      gameEventOnClick: GameActions.ResetDiceEvent()
    });
    if (!state.EnergyDice?.AssignedSkills?.some(x => x == null))
    {
      availableButtons.push({
        text: "Confirm Dice Assignment",
        gameEventOnClick: GameActions.ConfirmDiceAssignmentEvent()
      });
    }
  } 
  else if (state.GamePhase == "HeroActions")
  {
    availableButtons.push({
      text: "Reset Actions",
      gameEventOnClick: GameActions.HeroActionResetEvent()
    });

    availableButtons.push({
      text: "End Actions",
      gameEventOnClick: GameActions.HeroActionEndEvent()
    });
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
      text: "Upgrade Movement",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Movement")
    });
    availableButtons.push({
      text: "Upgrade Attack",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Attack")
    });
    availableButtons.push({
      text: "Upgrade Defence",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("Defence")
    });
    availableButtons.push({
      text: "Upgrade Range",
      gameEventOnClick: GameActions.NextLevelUpgradeSkillEvent("AttackRange")
    });
    availableButtons.push({
      text: "Replenish Health",
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
