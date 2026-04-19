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
        try {
            const jsonPayload = JSON.stringify(payload);
            const newStateJson : string = await window.DotNet.invokeMethodAsync('DungeonGame.Wasm', 'ProcessInput', jsonPayload);
            const newState = JSON.parse(newStateJson) as GameState;

            // analytics event
            if (typeof gtag === 'function')
            {
                LogGameEvents(state, newState);
            }

            setState(newState);
        } catch (error: any) {
            gtag('event', 'engine_error', {
                'error_message': error instanceof Error ? error.message : 'Unknown Wasm Error',
                'current_state': state,
                'last_input': payload,
                'fatal': true 
            });

            console.log(error);
            alert(error);
        }
    };

    // TODO, loadGameState, saveGameState

    return { state, dispatch, isReady };
}

const LogGameEvents = (state: GameState, newState: GameState): void => {
    if (newState.GamePhase === "GameEnd")
    {
        // game won
        if (newState.Hero!.Health! > 0)
        {
            const hasHopeBonus = newState.World!.Monsters!.some(x => x.Monster.Type === "Hope");

            gtag('event', 'game_won', { 
                'level_number': newState.LevelNumber, 
                'stat_movement': newState.Hero!.Stats!["Movement"], 
                'stat_attack': newState.Hero!.Stats!["Attack"], 
                'stat_defence': newState.Hero!.Stats!["Defence"],
                'stat_range': newState.Hero!.Stats!["AttackRange"],
                'lives': newState.LevelRetriesAvailable! + 1,
                'health': newState.Hero!.Health!,
                'hope_bonus': hasHopeBonus ? 1 : 0,
                'score': (newState.LevelRetriesAvailable! + 1) + (newState.Hero!.Health!) + (hasHopeBonus ? 1 : 0)
            });
        }
        // game lost
        else 
        {
            gtag('event', 'game_lost', { 
                'level_number': newState.LevelNumber, 
                'lives': newState.LevelRetriesAvailable!,
                'stat_movement': newState.Hero!.Stats!["Movement"], 
                'stat_attack': newState.Hero!.Stats!["Attack"], 
                'stat_defence': newState.Hero!.Stats!["Defence"],
                'stat_range': newState.Hero!.Stats!["AttackRange"] 
            });
        }
    }
    // next level
    else if (((newState.LevelNumber ?? 0)) > (state.LevelNumber ?? 0))
    {
        gtag('event', 'next_level', { 
            'level_number': newState.LevelNumber, 
            'lives': newState.LevelRetriesAvailable! + 1 
        });
    }
};