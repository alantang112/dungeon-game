interface GameLogProps {
  messages: string[];
}

export const GameLog = ({ messages }: GameLogProps) => {
  return (
    <div className="w-full pointer-events-none z-15">
    <div className="mx-auto bg-slate-950/90 border border-slate-700 rounded-t-lg p-2 h-40 overflow-y-auto pointer-events-auto shadow-2xl backdrop-blur-sm">
      {messages.map((msg, i) => (
        <div key={i} className="text-sm font-mono text-slate-300">
          <span className="text-slate-600 mr-2">{'>'}</span>
          {" "}{msg}
        </div>
      ))}
    </div>
  </div>
  );
};