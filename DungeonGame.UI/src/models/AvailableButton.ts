import type { GameInputEvent } from "./GameEngineModels";

export interface AvailableButton {
  text: string;
  gameEventOnClick: GameInputEvent;
}