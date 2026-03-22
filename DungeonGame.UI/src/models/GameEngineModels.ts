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
    | "EnergyDiceConfirmAssignment"
    | "HeroActionMove"
    | "HeroActionAttack"
    | "HeroActionReset"
    | "HeroActionEnd"
    | "MonstersMove"
    | "MonstersAttack"
    | "MonsterActionsEnd"
    | "LevelEnd"
    | "GameEnd"
;

export type SkillType =
    | "Movement"
    | "Attack"
    | "Defence"
    | "AttackRange"
;

export type MonsterType =
    "Spider"
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
    HeroActionPoints?: Map<SkillType, number>;
    Walls?: Position[];
    Monsters?: MonsterPosition[];
}

export class Hero {
    Name?: string;
    Health?: number;
    Stats?: Map<SkillType, number>;
}

export class Monster {
    Type!: MonsterType;
    Health!: number;
    Stats!: Map<SkillType, number>;
}

export class MonsterPosition {
    Monster!: Monster;
    Position!: Position;
}

export class GameState {
    GamePhase!: GamePhase;
    EnergyDice?: EnergyDice;
    Hero?: Hero;
    World?: World;
    LevelNumber?: number;
    GameMessage?: string;
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
}

export class GameInputEvent {
    EventType?: GameInputEventType;
    EventParameters?: GameInputEventParameters;
}