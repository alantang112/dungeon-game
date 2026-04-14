import { GameInputEvent, type SkillType } from "../models/GameEngineModels";

const NewGameEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "NewGame"
    };

    return model;
};

const RollDiceEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "EnergyDiceRoll"
    };

    return model;
}

const AssignDiceEvent = (diceIndex: number, skillType: SkillType) : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "EnergyDiceAssign",
        EventParameters: {
            DiceIndex: diceIndex,
            SkillType: skillType
        }
    };

    return model;
}

const ResetDiceEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "EnergyDiceResetAssignment"
    };

    return model;
}

const RerollDiceEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "EnergyDiceReroll"
    };

    return model;
}

const HeroMoveEvent = (x: number, y: number) : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "HeroActionMove",
        EventParameters: {
            X: x,
            Y: y
        }
    };

    return model;
}

const HeroAttackEvent = (x: number, y: number) : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "HeroActionAttack",
        EventParameters: {
            X: x,
            Y: y
        }
    };

    return model;
}

const HeroActionResetEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "HeroActionReset"
    };

    return model;
}

const HeroActionEndEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "HeroActionEnd"
    };

    return model;
}

const MonsterActionEndEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "MonsterActionsEnd"
    };

    return model;
}

const NextLevelUpgradeSkillEvent = (skillType: SkillType) : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "NextLevel",
        EventParameters: {
            SkillType: skillType,
            ReplenishHealth: false
        }
    }

    return model;
}

const NextLevelReplenishHealthEvent = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "NextLevel",
        EventParameters: {
            ReplenishHealth: true
        }
    }

    return model;
}

const RetryLevel = () : GameInputEvent => {
    const model: GameInputEvent = {
        EventType: "RetryLevel",
    };

    return model;
}

const interfaceActions = { 
    NewGameEvent, 
    RollDiceEvent, 
    AssignDiceEvent, 
    ResetDiceEvent, 
    RerollDiceEvent,
    HeroMoveEvent,
    HeroAttackEvent,
    HeroActionResetEvent,
    HeroActionEndEvent,
    MonsterActionEndEvent,
    NextLevelUpgradeSkillEvent,
    NextLevelReplenishHealthEvent,
    RetryLevel
 }

 export default interfaceActions;

