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
    <div className="flex flex-col h-40 w-full max-w-md bg-slate-950 border border-slate-700 rounded p-2">
      <div className="overflow-y-auto pr-2 scrollbar-thin scrollbar-thumb-slate-700">
        {messages.map((msg, i) => (
          <div key={i} className="text-sm text-slate-300 font-mono mb-1">
            <span className="text-slate-500 mr-2">[{i}]</span>
            {msg}
          </div>
        ))}
        {/* Invisible element to anchor the scroll */}
        <div ref={logEndRef} />
      </div>
    </div>
  );
};