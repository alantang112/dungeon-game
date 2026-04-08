import type { TileType } from "./TileType";

export class TileData {
    tileType!: TileType;
    health?: number;
    maxHealth?: number;
    name?: string;
    isMonster!: boolean;
    monsterId?: string;
    shouldPulseDarken?: boolean;
}