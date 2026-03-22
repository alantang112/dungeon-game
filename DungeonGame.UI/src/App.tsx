import { useState } from 'react';
import { useGameEngine } from './hooks/useGameEngine';
import { GameInputEvent, MonsterPosition, Position, GameState } from './models/GameEngineModels'
import { Tile } from './props/Tile';
import { GameLog } from './props/GameLog';
import GameActions from './interfaces/gameEngineInterface';
import './App.css'

function App() {
  const { state, dispatch, isReady } = useGameEngine();
  const [jsonInput, setJsonInput] = useState(`{
    "EventType": "NewGame",
    "EventParameters": {
        "HeroName": "Lil Timmy"
    }
}`);

  const handleDispatch = async () => {
    try {
      const inputModel : GameInputEvent = JSON.parse(jsonInput) as GameInputEvent; 
      await dispatch(inputModel);
    } catch (e) {
      console.log(jsonInput, e);
      alert("Could not process input event!");
    }
  }

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
      console.log(jsonInput, e);
      alert("Could not process input event!");
    }
  }

  const gridSize : number = 5;
  const gridRows = [];

  for (let y = gridSize; y >= 1; y--) {
    for (let x = 1; x <= gridSize; x++) {
      gridRows.push(
        <div onClick={() => handleTileClick(state, x, y)}>
          <Tile
          key={`${x}-${y}`}
          x={x}
          y={y}
          backgroundColor={`${getTileBackgroundColor(state, x, y)}`}
          />
        </div>
      );
    }
  }

  return (
    <>
      <div className="absolute top-0 left-0 w-full h-[350px] flex p-5 gap-5 font-mono">
        {/* LEFT SIDE: INPUT */}
        <div className="flex-1 flex flex-col gap-2">
          <h3>Engine Input (JSON)</h3>
          <textarea
            value={jsonInput}
            onChange={(e) => setJsonInput(e.target.value)}
            className="h-full bg-[#1e1e1e] text-[#d4d4d4] p-2 resize-none"
          />
          <button 
            onClick={handleDispatch}
            className="p-2.5 cursor-pointer bg-[#007acc] text-white border-none hover:bg-[#005fa3]"
          >
            Dispatch to C# Engine
          </button>
        </div>

        {/* RIGHT SIDE: OUTPUT */}
        <div className="flex-1 flex flex-col gap-2 overflow-hidden">
          <h3>Current Game State</h3>
          <pre className="h-full bg-[#f4f4f4] border border-gray-300 overflow-auto p-2 text-xs">
            {state ? getGameStateJson(state) : ""}
          </pre>
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
  );
}

const getTileBackgroundColor = (state: GameState, x: number, y: number) : string => {
    if (state?.World?.HeroPosition?.X === x && state?.World?.HeroPosition?.Y === y) return "green";
    if (state?.World?.Walls?.some((w: Position) => w.X === x && w.Y === y)) return "darkgrey";
    if (state?.World?.Monsters?.some((m: MonsterPosition) => m.Position.X === x && m.Position.Y === y)) return "darkred";
    return "";
};

const getGameStateJson = (state: GameState) : string => {
  const stateJson = JSON.parse(JSON.stringify(state));

  if (stateJson?.World?.Borders)
  {
    stateJson.World.Borders = null;
    stateJson.World.Walls = null;
  }

  return JSON.stringify(stateJson, null, 2);
}

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

  return availableButtons;
}

export default App
