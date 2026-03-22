import { useState } from 'react';
import { useGameEngine } from './hooks/useGameEngine';
import { GameInputEvent, MonsterPosition, Position } from './models/GameEngineModels'
import { Tile } from './Tile';
import './App.css'

function App() {
  const { state, dispatch, isReady } = useGameEngine();
  const [jsonInput, setJsonInput] = useState(`{
    "EventType": "NewGame",
    "EventParameters": {
        "HeroName": "Bob"
    }
}`);

  const handleDispatch = async () => {
    try {
      const inputModel: GameInputEvent = JSON.parse(jsonInput) as GameInputEvent; 
      await dispatch(inputModel);
    } catch (e) {
      console.log(jsonInput, e);
      alert("Could not process input event!");
    }
  }

  if (!isReady) return <div>Loading game engine...</div>;

  const gridSize = 5;
  const gridRows = [];

  const getTileBackgroundColor = (x : number, y : number) => {
        if (state?.World?.HeroPosition?.X == x && state?.World?.HeroPosition?.Y == y)
            return "green";

        if (state?.World?.Walls?.some((wall: Position) => wall.X == x && wall.Y == y))
            return "darkgrey"

        if (state?.World?.Monsters?.find((mp: MonsterPosition) => mp.Position.X == x && mp.Position.Y == y))
            return "darkred";

        return "";
    };

  for (let y = gridSize; y >= 1; y--) {
    for (let x = 1; x <= gridSize; x++) {


      gridRows.push(
        <Tile
          key={`${x}-${y}`}
          x={x}
          y={y}
          backgroundColor={`${getTileBackgroundColor(x, y)}`}
        />
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
            {state ? JSON.stringify(state, null, 2) : ""}
          </pre>
        </div>

      </div>
      <div className="flex flex-col items-center justify-center min-h-screen bg-slate-900 p-4">
        <div 
          className="grid grid-cols-5 gap-1 w-full max-w-md border-2 border-slate-600 bg-slate-700 p-1"
          style={{ gridTemplateColumns: `repeat(${gridSize}, minmax(0, 1fr))` }}
        >
          {gridRows}
        </div>
      </div>
    </>
  );
}

export default App
