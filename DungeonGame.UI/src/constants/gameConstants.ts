import type { SkillType } from "../models/GameEngineModels";

export const DebugMode: boolean = true; 

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