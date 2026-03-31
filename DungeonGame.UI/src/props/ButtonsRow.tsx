import type { AvailableButton } from "../models/AvailableButton";

interface ButtonsRowProps {
    buttons: AvailableButton[],
    eventDispatcher: any
}

export const ButtonsRow = ({ buttons, eventDispatcher }: ButtonsRowProps) => {
  return (
    <>
        {buttons.map((button: AvailableButton, index: number) => (
        <button
            key={index}
            onClick={async (e) => {
                if (button.onClick)
                {
                    await button.onClick(e);
                }

                if (button.gameEventOnClick)
                {
                    await eventDispatcher(button.gameEventOnClick);
                }
            }}
            className={`
        
            inline-block
            px-7 py-3
            text-white bg-indigo-600
            font-bold tracking-wide
            rounded-full shadow-[0_10px_20px_rgba(0,0,0,0.4)] 
            transform transition-all active:scale-95
            border border-indigo-400/30 
        ` + (button.disabled === true ? 'opacity-60 ' : 'hover:bg-indigo-500  ')}
        >
            {button.textNode}
        </button>
        ))}
    </>
  );
};
