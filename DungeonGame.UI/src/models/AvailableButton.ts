import type { ReactNode } from "react";
import type { GameInputEvent } from "./GameEngineModels";

export interface AvailableButton {
  textNode: ReactNode;
  gameEventOnClick: GameInputEvent;
}