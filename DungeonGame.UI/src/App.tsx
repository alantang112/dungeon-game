import { useState } from 'react';
import { useGameEngine } from './hooks/useGameEngine';
import './App.css'

function App() {
  const { state, dispatch, isReady } = useGameEngine();
  const [jsonInput, setJsonInput] = useState('');

  const handleDispatch = async () => {
    try {
      const payload = JSON.parse(jsonInput);
      await dispatch(payload);
    } catch (e) {
      alert("Invalid JSON format. Please check your syntax.");
    }
  }

  if (!isReady) return <div>Loading game engine...</div>;

  return (
    <div style={{ display: 'flex', gap: '20px', padding: '20px', fontFamily: 'monospace' }}>
      
      {/* LEFT SIDE: INPUT */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
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
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', gap: '10px' }}>
        <h3>Current Game State</h3>
        <pre style={{ 
          backgroundColor: '#f4f4f4', 
          border: '1px solid #ccc', 
          height: '350px', 
          overflow: 'auto' 
        }}>
          {state ? JSON.stringify(state, null, 2) : ""}
        </pre>
      </div>

    </div>
  );
}

export default App
