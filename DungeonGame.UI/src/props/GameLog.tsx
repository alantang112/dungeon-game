import { useEffect, useRef } from 'react';

interface GameLogProps {
  messages: string[];
}

export const GameLog = ({ messages }: GameLogProps) => {
  const logEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    logEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  return (
    <div className="fixed bottom-0 left-0 w-full p-[10px] pointer-events-none z-15">
    <div className="mx-auto bg-slate-950/90 border border-slate-700 rounded-t-lg p-2 h-40 overflow-y-auto pointer-events-auto shadow-2xl backdrop-blur-sm">
      {messages.map((msg, i) => (
        <div key={i} className="text-sm font-mono text-slate-300">
          <span className="text-slate-600 mr-2">{'>'}</span>
          {" "}{msg}
        </div>
      ))}
      <div ref={logEndRef} />
    </div>
  </div>
  );
};