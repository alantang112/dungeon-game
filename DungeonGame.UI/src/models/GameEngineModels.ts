export type GamePhase = 
    "Start" 
    | "EnergyDicePreRoll" 
    | "EnergyDiceAssignment" 
    | "HeroActions" 
    | "MonsterActions" 
    | "LevelEnd" 
    | "GameEnd"
;

export type GameInputEventType = 
    "NewGame"
    | "EnergyDiceRoll"              
    | "EnergyDiceAssign"
    | "EnergyDiceResetAssignment"
    | "EnergyDiceReroll"
    | "HeroActionMove"
    | "HeroActionAttack"
    | "HeroActionReset"  
    | "HeroActionEnd"    
    | "MonsterActionsEnd"
    | "NextLevel"
    | "GameEnd"
;

export type SkillType =
    "Movement"
    | "Attack"
    | "Defence"
    | "AttackRange"
;

export type MonsterType =
    "Spider"
    | "Skeleton"
    | "Minotaur"
    | "Fiendling"
    | "Colossus"
    | "Overseer"
;

export type DiceType =
    "D4"
  | "D6"
;

export class EnergyDice {
    Dice?: number[];
    AssignedSkills?: (SkillType | null)[];
}

export class Position {
    X!: number;
    Y!: number;
}

export class World {
    HeroPosition!: Position;
    HeroActionPoints?: Record<SkillType, number>;
    RerollsAvailable?: number;
    Walls?: Position[];
    Monsters?: MonsterPosition[];
}

export class Hero {
    Name?: string;
    Health?: number;
    Stats?: Record<SkillType, number>;
}

export class Monster {
    Id!: string;
    Type!: MonsterType;
    Name!: string;
    Health!: number;
    MaxHealth!: number;
    Stats!: Record<SkillType, number>;
    IsBossType!: boolean;
    BossDice!: Record<SkillType, number>;
    BossDiceType?: DiceType;
}

export class MonsterPosition {
    Monster!: Monster;
    Position!: Position;
    LastMovementPath?: Position[];
}

export class ViewData {
    HeroCanWalkPositions?: Position[];
    HeroCanAttackPositions?: Position[];
    MonstersAttacking?: string[];
    MonsterAttackedByHero?: MonsterPosition;
}

export class GameState {
    GamePhase!: GamePhase;
    EnergyDice?: EnergyDice;
    Hero?: Hero;
    World?: World;
    LevelNumber?: number;
    GameMessage?: string;
    GameMessageLog?: string[];
    ViewData?: ViewData;
}

export class GameInputEventParameters {
    // NewGame
    HeroName?: string;

    // EnergyDiceAssign
    DiceIndex?: number;
    SkillType?: SkillType;

    // HeroMove/HeroAttack
    X?: number;
    Y?: number;

    // NextLevel
    // SkillType
    ReplenishHealth?: boolean;
}

export class GameInputEvent {
    EventType?: GameInputEventType;
    EventParameters?: GameInputEventParameters;
}