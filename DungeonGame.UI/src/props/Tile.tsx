import { useEffect, useRef, useState } from "react";
import type { TileType } from "../models/TileType";
import { assetPath } from "../constants/assetConstants";
import { damageAnimationDelayMs } from "../constants/gameConstants";

interface TileProps {
    x: number;
    y: number;
    ghostTileType?: TileType;
    tileType: TileType;
    name: string;
    heroCanWalk: boolean;
    heroCanAttack: boolean;
    health?: number;
    maxHealth?: number;
    isMonster: boolean;
    isAttacking?: boolean;
    attackId?: string;
    angleToTarget?: number;
    pulse?: boolean;
    customBgColor?: string;
    customAttackRingColor?: string;
}

export const Tile = ({ x, y, ghostTileType, tileType, name, heroCanWalk, heroCanAttack, health, maxHealth, 
    isMonster, isAttacking, attackId, angleToTarget, pulse, customBgColor, customAttackRingColor }: TileProps) => {
    // Damage display logic
    const [isAnimatingDamage, setIsAnimatingDamage] = useState(false);
    const identity = (name.length > 0) ? (tileType + name) : undefined;
    const prevIdentityRef = useRef<string | undefined>(identity);
    const prevHealthRef = useRef<number | undefined>(health);

    useEffect(() => {
        if (ghostTileType) {
            const damageTaken = 1;
            const totalDuration = (damageTaken * 250) + damageAnimationDelayMs; 

            setIsAnimatingDamage(true);

            const timer = setTimeout(() => {
                setIsAnimatingDamage(false);
            }, totalDuration);

            return () => {
                clearTimeout(timer);
            };
        }
        else {
            const entityChanged = prevIdentityRef.current !== identity && prevIdentityRef.current !== undefined && identity !== undefined;

            const shouldFlash = identity !== undefined &&
                                !entityChanged && 
                                prevHealthRef.current !== undefined && 
                                health !== undefined && 
                                health < prevHealthRef.current;

            if (shouldFlash) {
                const damageTaken = prevHealthRef.current! - health!;
                const totalDuration = (damageTaken * 250) + damageAnimationDelayMs; 

                setIsAnimatingDamage(true);

                const timer = setTimeout(() => {
                    setIsAnimatingDamage(false);
                }, totalDuration);

                return () => {
                    clearTimeout(timer);
                };
            } else {
                setIsAnimatingDamage(false);
            }
        }
    }, [ghostTileType, health, identity]);

    useEffect(() => {
        prevHealthRef.current = health;
        prevIdentityRef.current = identity;
    }); // runs on every render

    // Attack display logic
    const shouldFlip = isMonster ? (angleToTarget !== undefined && angleToTarget > -Math.PI/2 && angleToTarget < Math.PI/2) : false;

    const tileStyle: React.CSSProperties = {
        ...(angleToTarget !== undefined && { 
            "--bump-angle": `${angleToTarget}rad`,
            "--flip-factor": shouldFlip ? -1 : 1, 
        } as React.CSSProperties)
    };

    // "Ghost tile" logic
    // 1. Initialize state: Use ghostTileType if it exists, otherwise the regular tileType
    const [displayType, setDisplayType] = useState<TileType>(ghostTileType ?? tileType);

    useEffect(() => {
        if (ghostTileType) {
            const duration = damageAnimationDelayMs + 50;
                        
            const swapTimer = setTimeout(() => {
                setDisplayType(tileType);
            }, duration);

            return () => {
                clearTimeout(swapTimer);
            };
        }
        else {
            setDisplayType(tileType);
        }
    }, [ghostTileType, tileType]);

    return (
        <div className={"flex items-center justify-center border border-slate-700 text-slate-500 text-xs w-full h-16 sm:h-22 aspect-square relative "
            + (isAnimatingDamage ? 'animate-damage ' : '')
            + (heroCanWalk ? 'border-2 border-transparent shadow-[inset_0_0_0_2px_#bababa] ' : '')
            + (heroCanAttack ? (customAttackRingColor ? customAttackRingColor : 'border-2 border-transparent shadow-[inset_0_0_0_2px_#fa6b6b] ') : '')
            + ((heroCanWalk || heroCanAttack) ? 'cursor-pointer ' : '')
            + (customBgColor ? customBgColor : 'bg-slate-800 ')}
            >
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
            {displayType != "Empty" && (
                <img
                    key={`${x}-${y}-${displayType}-${attackId}`}
                    src={`${assetPath}${displayType.toLowerCase()}.png`}
                    alt={`Tile ${x}-${y}`}
                    className={'w-full h-full object-contain p-1 z-10 ' 
                        + (isAttacking ? 'animate-bump ': '')
                        + (shouldFlip ? '-scale-x-100 ' : 'scale-x-100 ')
                        + (pulse ? 'pulse-hue ' : '')}
                    style={tileStyle}
                />
            )}
        </div>
    )
};
