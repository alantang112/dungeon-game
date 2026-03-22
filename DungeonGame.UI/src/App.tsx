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
      <div style={{ position: 'absolute', height: '350px', width: '100vw', display: 'flex', gap: '20px', padding: '20px', fontFamily: 'monospace' }}>
        
        {/* LEFT SIDE: INPUT */}
        <div style={{ height: '100%', flex: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
          <h3>Engine Input (JSON)</h3>
          <textarea
            value={jsonInput}
            onChange={(e) => setJsonInput(e.target.value)}
            style={{ height: '300px', backgroundColor: '#1e1e1e', color: '#d4d4d4' }}
          />
          <button 
            onClick={handleDispatch}
            style={{ padding: '10px', cursor: 'pointer', backgroundColor: '#007acc', color: 'white', border: 'none' }}
          >
            Dispatch to C# Engine
          </button>
        </div>

        {/* RIGHT SIDE: OUTPUT */}
        <div style={{ height: '100%', flex: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
          <h3>Current Game State</h3>
          <pre style={{ 
            backgroundColor: '#f4f4f4', 
            border: '1px solid #ccc', 
            overflow: 'auto' 
          }}>
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

export default App
