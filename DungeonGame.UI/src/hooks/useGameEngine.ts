import { useState, useEffect } from 'react';

export function useGameEngine() {
    const [state, setState] = useState<any>(null);
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

                    const newState = await window.DotNet.invokeMethodAsync('DungeonGame.Wasm', 'Initialize', null);
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

    const dispatch = async (payload: any) => {
        const newState = await window.DotNet.invokeMethodAsync('DungeonGame.Wasm', 'ProcessInput', payload);
        setState(newState);
    };

    // TODO, loadGameState, saveGameState

    return { state, dispatch, isReady };
}