import type { SkillType } from "../models/GameEngineModels";

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

export const howToPlayUrl = 'https://www.google.com/search?q=one+card+dungeon+rulebook';

export const levelNameColor = (levelNumber: number): string => {
    switch (levelNumber) {
        case 1:
            return 'text-mist-50';
        case 2:
            return 'text-amber-200';
        case 3:
            return 'text-amber-300';
        case 4:
            return 'text-amber-400';
        case 5:
            return 'text-amber-500';
        case 6:
            return 'text-amber-600';            
        case 7:
            return 'text-red-400';
        case 8:
            return 'text-red-500';
        case 9:
            return 'text-red-600';
        case 10:
            return 'text-red-700';
        case 11:
            return 'text-purple-400';
        case 12:
            return 'text-purple-500';
        case 13:
            return 'text-purple-600';
        default:
            return '';
    }        
}

export const damageAnimationDelayMs: number = 180;