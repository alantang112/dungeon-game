import { assetPath } from "../constants/assetConstants";

interface DiceProps {
    number: number;
    active: boolean;
    disabled: boolean;
}

export const Dice = ({ number, active, disabled }: DiceProps) => {
    return (
        <div className="flex items-center justify-center w-full h-12 aspect-square relative">
            <img
            src={`${assetPath}dice${number}.png`}
            alt={`Dice ${number}`}
            className={`
                    w-full h-full object-contain p-1 transition-all duration-200
                    ${active ? "drop-shadow-[0_0_8px_rgba(255,255,255,0.8)] scale-110" : ""}
                    ${disabled ? "brightness-50 grayscale-[0.2]" : ""}
                `}
            />
        </div>
    )
};