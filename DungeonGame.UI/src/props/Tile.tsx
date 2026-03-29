import type { TileType } from "../models/TileType";

interface TileProps {
    x: number;
    y: number;
    tileType: TileType;
}

export const Tile = ({ x, y, tileType }: TileProps) => {
    return (
        <div className="flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-22 aspect-square relative">
            <span className="absolute top-1 left-1 opacity-80">{x},{y}</span>
            {tileType != "Empty" && (
                <img
                    src={`assets/${tileType.toLowerCase()}.png`}
                    alt={`Tile ${x}-${y}`}
                    className="w-full h-full object-contain p-1 z-10"
                />
            )}
        </div>
    )
};