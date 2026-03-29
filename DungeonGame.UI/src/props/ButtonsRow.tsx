import type { AvailableButton } from "../models/AvailableButton";

interface ButtonsRowProps {
    buttons: AvailableButton[],
    eventDispatcher: any
}

export const ButtonsRow = ({ buttons, eventDispatcher }: ButtonsRowProps) => {
  return (
    <div className="flex gap-6 mt-15 h-12 items-center justify-center">
              {buttons.map((button: AvailableButton, index: number) => (
                <button
                  key={index}
                  onClick={async () => await eventDispatcher(button.gameEventOnClick)}
                  className="
              
                  inline-block
                  px-7 py-3
                  bg-indigo-600 hover:bg-indigo-500 
                  text-white font-bold tracking-wide
                  rounded-full shadow-[0_10px_20px_rgba(0,0,0,0.4)] 
                  transform transition-all active:scale-95
                  border border-indigo-400/30
                "
                >
                  {button.text}
                </button>
              ))}
            </div>
  );
};
