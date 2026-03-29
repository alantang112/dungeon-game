import { useState, useEffect } from 'react';
import { GameState, GameInputEvent } from '../models/GameEngineModels';

export function useGameEngine() {
    const [state, setState] = useState<GameState>(new GameState());
    const [isReady, setIsReady] = useState(false);

    useEffect(() => {
        const init = async () => {
            while (!window.Blazor) {
                await new Promise(r => setTimeout(r, 50));
            }

            try {
                if (!window.IsBlazorStarted) {
                    window.IsBlazorStarted = true;
                    await window.Blazor.start();
                    console.log("🚀 .NET Engine Started");

                    await new Promise(r => setTimeout(r, 150));

                    const newStateJson : string = await window.DotNet.invokeMethodAsync('DungeonGame.Wasm', 'Initialize', null);
                    const newState = JSON.parse(newStateJson) as GameState;
                    setState(newState);
                    setIsReady(true);

                    console.log("🤖 Game engine ready");
                }

                setIsReady(true);
            } catch (err: any) {
                if (err.message?.includes("already started")) {
                    setIsReady(true);
                } else {
                    console.error("Blazor Start Error:", err);
                }
            }
        };
        init();
    }, []);

    const dispatch = async (payload: GameInputEvent) => {
        const jsonPayload = JSON.stringify(payload);
        console.log(jsonPayload);
        const newStateJson : string = await window.DotNet.invokeMethodAsync('DungeonGame.Wasm', 'ProcessInput', jsonPayload);
        console.log(newStateJson);
        const newState = JSON.parse(newStateJson) as GameState;
        setState(newState);
    };

    // TODO, loadGameState, saveGameState

    return { state, dispatch, isReady };
}