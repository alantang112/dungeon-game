import { useEffect, useRef, useState } from "react";
import type { TileType } from "../models/TileType";
import { assetPath } from "../constants/assetConstants";

interface TileProps {
    x: number;
    y: number;
    tileType: TileType;
    name: string;
    heroCanWalk: boolean;
    heroCanAttack: boolean;
    health?: number;
    maxHealth?: number;
}

export const Tile = ({ x, y, tileType, name, heroCanWalk, heroCanAttack, health, maxHealth }: TileProps) => {
    const [isAnimatingDamage, setIsAnimatingDamage] = useState(false);
    const identity = tileType + name;
    const prevIdentity = useRef<string | undefined>(identity);
    const prevHealthRef = useRef<number | undefined>(health);

    useEffect(() => {
        // Only trigger if health actually decreased, and same entity on the tile
        if (prevHealthRef.current !== undefined && health !== undefined && health < prevHealthRef.current
            && prevIdentity.current !== undefined && identity !== undefined && prevIdentity.current === identity) {
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
        prevIdentity.current = identity;
    }, [health]);

    return (
        <div className={"flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-16 sm:h-22 aspect-square relative "
            + (isAnimatingDamage ? 'animate-damage ' : 'bg-slate-800 ')
            + (heroCanWalk ? 'border-2 border-transparent shadow-[inset_0_0_0_2px_#bababa] ' : '')
            + (heroCanAttack ? 'border-2 border-transparent shadow-[inset_0_0_0_2px_#fa6b6b] ' : '')
            + ((heroCanWalk || heroCanAttack) ? 'cursor-pointer ' : '')}>
            {
                health !== undefined &&
                <div className="absolute top-1 right-0 w-full flex px-1">
                <div className="flex-auto flex flex-col">
                    <div className="flex items-center gap-3">
                        <div className="flex h-4 flex-1">
                        {[...Array(maxHealth)].map((_, i) => (
                            <div
                            key={i}
                            className={`h-[5px] flex-1 transition-colors duration-500 border border-slate-900 ${
                                i < health! ? ' bg-green-500 ' : ' bg-red-700 '
                            }`}
                            />
                        ))}
                        </div>
                    </div>
                </div>
            </div>
            }
            
            <span className="absolute bottom-0 left-1 opacity-70 text-white z-15">{name}</span>
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