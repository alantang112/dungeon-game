import { useEffect, useState } from "react";
import { assetPath, diceFileName } from "../constants/assetConstants";
import { diceShuffleSpeed } from "../constants/gameConstants";

export const ShufflingDice = () => {
    const [displayNumber, setDisplayNumber] = useState(1);

    useEffect(() => {
        const interval = setInterval(() => {
            setDisplayNumber(prev => {
                const next = Math.floor(Math.random() * 6) + 1;
                return next === prev ? (next % 6) + 1 : next;
            });
        }, diceShuffleSpeed);

        return () => clearInterval(interval);
    });

    return (
        <div className="flex items-center justify-center w-full h-12 aspect-square relative">
            <img
                src={`${assetPath}${diceFileName}${displayNumber}.png`}
                alt="Shuffling Dice"
                className={`
                    w-full h-full object-contain p-1
                `}
            />
        </div>
    );
};