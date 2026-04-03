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
    const identity = (name.length > 0) ? (tileType + name) : undefined;
    const prevIdentityRef = useRef<string | undefined>(identity);
    const prevHealthRef = useRef<number | undefined>(health);

    useEffect(() => {
        const entityChanged = prevIdentityRef.current !== identity;

        const shouldFlash = !entityChanged && 
                           prevHealthRef.current !== undefined && 
                           health !== undefined && 
                           health < prevHealthRef.current;

        if (shouldFlash) {
            const damageTaken = prevHealthRef.current! - health!;
            const totalDuration = damageTaken * 250; 

            setIsAnimatingDamage(true);

            const timer = setTimeout(() => {
                setIsAnimatingDamage(false);
            }, totalDuration);

            return () => {
                clearTimeout(timer);
            };
        } else if (entityChanged) {
            setIsAnimatingDamage(false);
        }

        prevHealthRef.current = health;
        prevIdentityRef.current = identity;
    }, [health, identity]);

    return (
        <div className={"flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-16 sm:h-22 aspect-square relative "
            + (isAnimatingDamage ? 'animate-damage ' : '')
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
                            className={`h-[4px] flex-1 transition-colors duration-500 border-x-1 border-slate-700 ${
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
