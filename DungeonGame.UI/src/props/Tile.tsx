import { useEffect, useRef, useState } from "react";
import type { TileType } from "../models/TileType";
import { assetPath } from "../constants/assetConstants";

interface TileProps {
    x: number;
    y: number;
    tileType: TileType;
    heroCanWalk: boolean;
    heroCanAttack: boolean;
    health?: number;
}

export const Tile = ({ x, y, tileType, heroCanWalk, heroCanAttack, health }: TileProps) => {
    const [isAnimatingDamage, setIsAnimatingDamage] = useState(false);
    const prevHealthRef = useRef<number | undefined>(health);

    useEffect(() => {
        // Only trigger if health actually decreased
        if (prevHealthRef.current !== undefined && health !== undefined && health < prevHealthRef.current) {
            const damageTaken = prevHealthRef.current - health;
            
            // Start the rapid pulse
            setIsAnimatingDamage(true);

            // Duration = 300ms per heart/hit point lost
            const totalDuration = damageTaken * 250; 

            const timer = setTimeout(() => {
                setIsAnimatingDamage(false);
            }, totalDuration);

            return () => clearTimeout(timer);
        }

        // Always update the ref so the next change is compared correctly
        prevHealthRef.current = health;
    }, [health]);

    return (
        <div className={"flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-22 aspect-square relative "
            + (isAnimatingDamage ? 'animate-damage ' : 'bg-slate-800 ')
            + (heroCanWalk ? 'border-2 border-transparent shadow-[inset_0_0_0_2px_#bababa] ' : '')
            + (heroCanAttack ? 'border-2 border-transparent shadow-[inset_0_0_0_2px_#fa6b6b] ' : '')
            + ((heroCanWalk || heroCanAttack) ? 'cursor-pointer ' : '')}>
            <span className="absolute top-1 left-1 opacity-80">{x},{y}</span>
            {tileType != "Empty" && (
                <img
                    src={`${assetPath}${tileType.toLowerCase()}.png`}
                    alt={`Tile ${x}-${y}`}
                    className='w-full h-full object-contain p-1 z-10'
                />
            )}
        </div>
    )
};