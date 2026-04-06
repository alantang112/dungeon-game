import type { HelpItem } from "../HowToPlayContent";

interface HelpPageRendererProps {
    items: HelpItem[];
}

export const HelpPageRenderer = ({ items }: HelpPageRendererProps) => {
    return (
        <div className="space-y-4"> {/* Vertical spacing between elements */}
            {items.map((item, index) => {
                switch (item.type) {
                    case 'heading':
                        return (
                            <h3 key={index} className="text-xl font-bold text-white mt-6 mb-2">
                                {item.content}
                            </h3>
                        );
                    case 'text':
                        return (
                            <p key={index} className="text-slate-300 leading-relaxed">
                                {item.content}
                            </p>
                        );
                    case 'ordered-list':
                        return (
                            <ol key={index} type='1' className="text-slate-300 leading-relaxed list-decimal list-inside">
                                {item.contents!.map(content => <li>{content}</li>)}
                            </ol>
                        );
                    case 'unordered-list':
                        return (
                            <ol key={index} type='1' className="text-slate-300 leading-relaxed list-disc list-inside">
                                {item.contents!.map(content => <li>{content}</li>)}
                            </ol>
                        );
                    case 'image':
                        return (
                            <div key={index} className="flex justify-center my-6 p-2 bg-slate-800 border border-slate-700 rounded">
                                <img
                                    src={item.content}
                                    alt={item.alt || "Help Image"}
                                    className="max-w-full object-contain" // Constrain image height
                                />
                            </div>
                        );
                    case 'separator':
                        return (
                            <hr key={index} className="border-slate-700 my-6" />
                        );
                    default:
                        return null;
                }
            })}
        </div>
    );
};