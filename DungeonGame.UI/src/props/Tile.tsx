interface TileProps {
    x: number;
    y: number;
    backgroundColor: string;
}

export const Tile = ({ x, y, backgroundColor }: TileProps) => {
    return (
        <div className="flex items-center justify-center border border-slate-700 bg-slate-800 text-slate-500 text-xs w-full h-22 aspect-square relative"
            style={{ backgroundColor: `${backgroundColor}` }}>
            <span className="absolute top-1 left-1 opacity-80">{x},{y}</span>
        </div>
    )
};