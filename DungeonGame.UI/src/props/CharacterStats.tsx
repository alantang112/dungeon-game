import { type SkillType } from '../models/GameEngineModels'; 
import { statColor, statText, colorEnergy } from '../constants/gameConstants';

interface CharacterStatsProps {
    name: string,
    stats?: Record<SkillType, number>,
    energy?: Record<SkillType, number>,
    displayEnergy: boolean,
    isEnemy: boolean
}

export const CharacterStats = ({ name, stats, energy, displayEnergy, isEnemy }: CharacterStatsProps) => {
  stats ??= {
    "Movement": 0,
    "Attack": 0,
    "Defence": 0,
    "AttackRange": 0
  };
  energy ??= {
    "Movement": 0,
    "Attack": 0,
    "Defence": 0,
    "AttackRange": 0
  };

  const containerBg = isEnemy
    ? "bg-red-950/40" // Darker, subtle red for enemies"
    : "bg-sky-950/40";  // Your original slate for heroes

  const headerColor = isEnemy ? "text-red-400" : "text-white";

  return (
    <div className={`${containerBg} p-2 mx-2 mb-2 rounded font-mono text-sm text-slate-200 flex gap-5 justify-between`}>
      <div className="flex items-start">
        <div className={`font-bold text-lg ml-3 mb-auto mx-auto ${headerColor}`}>
          {name}
        </div>
      </div>

      <div className={`grid grid-cols-4 ${isEnemy ? 'bg-red-900/30' : 'bg-slate-700'} border border-slate-700 rounded overflow-hidden mr-2 w-[220px]`}>
        
        {/* Header Row: Labels */}
        <div className={`${!isEnemy ? '' : 'hidden sm:block'} bg-slate-800 p-1 pr-2 text-center ${statColor["Movement"]} font-bold`}>{statText["Movement"]}</div>
        <div className={`${!isEnemy ? '' : 'hidden sm:block'} bg-slate-800 p-1 pr-2 text-center ${statColor["Attack"]} font-bold`}>{statText["Attack"]}</div>
        <div className={`${!isEnemy ? '' : 'hidden sm:block'} bg-slate-800 p-1 pr-2 text-center ${statColor["Defence"]} font-bold`}>{statText["Defence"]}</div>
        <div className={`${!isEnemy ? '' : 'hidden sm:block'} bg-slate-800 p-1 pr-2 text-center ${statColor["AttackRange"]} font-bold`}>{statText["AttackRange"]}</div>
        
        {/* Row 1: Base Values */}
        {
          <>
            {(["Movement", "Attack", "Defence", "AttackRange"] as SkillType[]).map((type) => (
              <FlashCell
                key={`base-${type}`}
                value={stats[type] ?? 0}
                className="bg-slate-900/20 p-1 text-center font-bold text-base"
                >{stats[type] ?? 0}</FlashCell>
            ))}
          </>
        }

        {/* Row 2: Energy */}
        {displayEnergy 
          ? <>
          {(["Movement", "Attack", "Defence"] as SkillType[]).map((type) => (
            <FlashCell
              key={`energy-${type}`}
              value={energy[type] ?? 0}
              className={`bg-slate-900/40 p-2 text-center ${colorEnergy} font-bold border-t border-slate-700`}
            >⚡{energy[type] ?? 0}</FlashCell>
          ))}
          <div className={`bg-slate-900/40 p-2 text-center text-slate-600`}>—</div>
          </> 
          : <></>}
      </div>
    </div>
  );
};

const FlashCell = ({ value, className, children }: { value: number; className: string; children: React.ReactNode }) => {
  return (
    <div className={`${className} relative overflow-hidden`}>
      {/* The Flash Layer */}
      <div 
        key={value} 
        className="absolute inset-0 pointer-events-none animate-flash"
      />
      
      {/* The Actual Content */}
      <span className="relative z-10">{children}</span>
    </div>
  );
};
