import { type SkillType } from '../models/GameEngineModels'; 
import { statColor, statText, colorEnergy } from '../constants/gameConstants';

interface CharacterStatsProps {
    name: string,
    health: number,
    maxHealth: number,
    stats?: Record<SkillType, number>,
    energy?: Record<SkillType, number>,
    displayEnergy: boolean,
    isEnemy: boolean
}

export const CharacterStats = ({ name, health, maxHealth, stats, energy, displayEnergy, isEnemy }: CharacterStatsProps) => {
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

  const greenBarsCount = health;

  const containerBg = isEnemy
    ? "bg-red-950/20 border-red-900/50" // Darker, subtle red for enemies"
    : "bg-slate-800 border-slate-600";  // Your original slate for heroes

  const headerColor = isEnemy ? "text-red-400" : "text-white";

  return (
    <div className={`${containerBg} p-4 rounded border border-slate-600 mb-4 font-mono text-sm text-slate-200`}>
      <div className="flex pb-3">
        <div className={`font-bold text-lg mt-auto ${headerColor}`}>
          {name}
        </div>
      </div>

      <div className={`grid grid-cols-5 gap-px ${isEnemy ? 'bg-red-900/30' : 'bg-slate-700'} border border-slate-700 rounded overflow-hidden`}>
        
        {/* Header Row: Labels */}
        <div className="bg-slate-900/80 p-2 text-[10px] text-slate-500 font-bold uppercase flex items-center">Type</div>
        <div className={`bg-slate-800 p-2 text-center ${statColor["Movement"]} font-bold`}>{statText["Movement"]}</div>
        <div className={`bg-slate-800 p-2 text-center ${statColor["Attack"]} font-bold`}>{statText["Attack"]}</div>
        <div className={`bg-slate-800 p-2 text-center ${statColor["Defence"]} font-bold`}>{statText["Defence"]}</div>
        <div className={`bg-slate-800 p-2 text-center ${statColor["AttackRange"]} font-bold`}>{statText["AttackRange"]}</div>

        {/* Row 1: Base Values */}
        <div className="bg-slate-900/50 p-2 text-xs text-slate-400 border-t border-slate-700">Base</div>
        <div className="bg-slate-900/20 p-2 text-center font-bold text-base">{stats["Movement"] ?? 0}</div>
        <div className="bg-slate-900/20 p-2 text-center font-bold text-base">{stats["Attack"] ?? 0}</div>
        <div className="bg-slate-900/20 p-2 text-center font-bold text-base">{stats["Defence"] ?? 0}</div>
        <div className="bg-slate-900/20 p-2 text-center font-bold text-base">{stats["AttackRange"] ?? 0}</div>

        {/* Row 2: Energy */}
        {displayEnergy 
          ? <>
          <div className={`bg-slate-900/50 p-2 text-xs ${colorEnergy}-500/80 border-t border-slate-700`}>Energy</div>
          {(["Movement", "Attack", "Defence"] as SkillType[]).map((type) => (
            <FlashCell
              key={type}
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