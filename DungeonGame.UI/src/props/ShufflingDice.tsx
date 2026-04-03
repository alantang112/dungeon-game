import { useEffect, useState } from "react";
import { assetPath, d4FileName, diceFileName } from "../constants/assetConstants";
import { diceShuffleSpeed } from "../constants/gameConstants";
import type { DiceType } from "../models/GameEngineModels";

interface ShufflingDiceProps {
    diceType: DiceType,
}

export const ShufflingDice = ({ diceType } : ShufflingDiceProps) => {
    const [displayNumber, setDisplayNumber] = useState(1);

    const maxDiceNumber = diceType === "D4" ? 4 : 6;
    const diceAsset = diceType === "D4" ? d4FileName : diceFileName; 

    useEffect(() => {
        const interval = setInterval(() => {
            setDisplayNumber(prev => {
                const next = Math.floor(Math.random() * maxDiceNumber) + 1;
                return next === prev ? (next % maxDiceNumber) + 1 : next;
            });
        }, diceShuffleSpeed);

        return () => clearInterval(interval);
    });

    return (
        <div className="flex items-center justify-center w-full h-12 aspect-square relative">
            <img
                src={`${assetPath}${diceAsset}${displayNumber}.png`}
                alt="Shuffling Dice"
                className={`
                    w-full h-full object-contain p-1
                `}
            />
        </div>
    );
};