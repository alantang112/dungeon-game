import type { SkillType } from "../models/GameEngineModels";

export const UIVersion: string = '1.8.0';

export const DebugMode: boolean = false; 

export const HeroMaxHealth: number = 6;
export const LevelSize: number = 5;
export const diceShuffleSpeed: number = 30;

export const colorEnergy: string = "text-yellow-400";

export const statColor: Record<SkillType, string> = {
    "Movement": "text-blue-300",
    "Attack": "text-red-400",
    "Defence": "text-emerald-400",
    "AttackRange": "text-purple-400"
}

export const statText: Record<SkillType, string> = {
    "Movement": "Move",
    "Attack": "Atk",
    "Defence": "Def",
    "AttackRange": "Range"
}

const monsterPathColors: string[] = [
  "#efc93d",
  "#ef7e3d",
  "#e24fe5",
  "#44b9d3",
];

export const getMonsterPathColor = (index: number): string => monsterPathColors[index];

export const levelNameColor = (levelNumber: number): string => {
    switch (levelNumber) {
        case 1:
        case 2:
        case 3:
            return 'text-mist-50';
        case 4:
        case 5:
        case 6:
            return 'text-amber-200';         
        case 7:
        case 8:
        case 9:
            return 'text-amber-300';
        case 10:
        case 11:
        case 12:
            return 'text-amber-400';
        case 13:
        case 14:
            return 'text-amber-500';
        case 15:
            return 'text-amber-600';
        case 16:
            return 'text-purple-500';  
        default:
            return '';
    }        
}

export const damageAnimationDelayMs: number = 180;

export const nightmareLevelnumber: number = 16;
export const borderWallCount: number = LevelSize * 4 + 4;