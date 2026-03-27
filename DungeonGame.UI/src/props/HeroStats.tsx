import { Hero, type SkillType } from '../models/GameEngineModels'; 

interface HeroStatsProps {
    hero?: Hero;
    energy?: Record<SkillType, number>
}

export const HeroStats = ({ hero, energy }: HeroStatsProps) => {
  const heroName: string = hero?.Name ?? "Hero";
  const health: number = hero?.Health ?? 10;
  const heroStats: Record<SkillType, number> = hero?.Stats ?? {
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

  const maxHealth = 10;
  const greenBarsCount = health;

  return (
    <div className="bg-slate-800 p-4 rounded border border-slate-600 mb-4 font-mono text-sm text-slate-200">
      <div className="mb-2 font-bold text-lg text-white border-b border-slate-700 pb-1">
        {heroName || "Hero"}
      </div>

      {/* Health Bar Row */}
      <div className="flex items-center gap-3 mb-4">
        <span className="w-16">Health:</span>
        <div className="flex gap-1 h-4 flex-1">
          {[...Array(maxHealth)].map((_, i) => (
            <div
              key={i}
              className={`h-full flex-1 rounded-sm transition-colors duration-500 ${
                i < greenBarsCount ? 'bg-green-500' : 'bg-red-600'
              }`}
            />
          ))}
        </div>
        <span className="text-xs w-8 text-right">{health}/{maxHealth}</span>
      </div>

      {/* Column Headers */}
      <div className="flex justify-between px-1 mb-1 text-[10px] uppercase text-slate-500 font-bold border-b border-slate-700/30">
        <span className="w-24">Stat</span>
        <span className="flex-1 text-center">Value</span>
        <span className="w-16 text-right text-yellow-500/80">Energy</span>
      </div>

      {/* Stats List */}
      <div className="flex flex-col gap-2 mt-2">
        <StatRow 
          label="Movement" 
          value={heroStats["Movement"]} 
          energy={energy["Movement"]} 
          color="text-blue-400" 
        />
        <StatRow 
          label="Attack" 
          value={heroStats["Attack"]} 
          energy={energy["Attack"]} 
          color="text-red-400" 
        />
        <StatRow 
          label="Defence" 
          value={heroStats["Defence"]} 
          energy={energy["Defence"]} 
          color="text-emerald-400" 
        />
        <StatRow 
          label="Range" 
          value={heroStats["AttackRange"]} 
          color="text-purple-400" 
        />
      </div>
    </div>
  );
};

// Updated internal helper for 3-column rows
const StatRow = ({ label, value, energy, color }: { label: string, value?: number, energy?: number, color: string }) => (
  <div className="flex justify-between items-center bg-slate-900/30 px-2 py-1.5 rounded-sm hover:bg-slate-700/30 transition-colors">
    <span className="w-24 text-slate-400">{label}:</span>
    <span className={`flex-1 text-center ${color} font-bold text-base`}>{value ?? 0}</span>
    <div className="w-16 text-right">
      {energy !== undefined ? (
        <span className="text-yellow-400 font-bold">⚡{energy}</span>
      ) : (
        <span className="text-slate-700">—</span> 
      )}
    </div>
  </div>
);