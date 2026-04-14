import type { ReactNode } from "react";
import type { GameInputEvent } from "./GameEngineModels";

export interface AvailableButton {
  textNode: ReactNode;
  gameEventOnClick?: GameInputEvent;
  onClick?: React.MouseEventHandler<HTMLButtonElement>;
  disabled?: boolean;
  smaller?: boolean;
  squareButton?: boolean;
}