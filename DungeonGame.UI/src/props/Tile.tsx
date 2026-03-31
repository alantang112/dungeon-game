import type { TileType } from "../models/TileType";
import { assetPath } from "../constants/assetConstants";

interface TileProps {
    x: number;
    y: number;
    tileType: TileType;
    heroCanWalk: boolean;
    heroCanAttack: boolean;
}

export const Tile = ({ x, y, tileType, heroCanWalk, heroCanAttack }: TileProps) => {
    return (
        <div className={"flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-22 aspect-square relative "
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