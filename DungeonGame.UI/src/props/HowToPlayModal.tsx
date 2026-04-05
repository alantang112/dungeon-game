import { useEffect, useRef, useState } from 'react';
import { HelpPageRenderer } from './HelpPageRenderer';
import { howToPlayPages } from '../HowToPlayContent';

interface HowToPlayModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export const HowToPlayModal = ({ isOpen, onClose }: HowToPlayModalProps) => {
    const dialogRef = useRef<HTMLDialogElement>(null);
    const [currentPageIndex, setCurrentPageIndex] = useState(0);

    useEffect(() => {
        const dialog = dialogRef.current;
        if (!dialog) return;

        if (isOpen) {
            // blocks background interaction
            dialog.showModal();
            // Reset to the first page when the modal opens
            setCurrentPageIndex(0); 
        } else {
            dialog.close();
        }
    }, [isOpen]);

    // 2. Prevent Closing when clicking outside
    // The <dialog> natively closes when you click on the backdrop (the area outside the dialog).
    // To prevent this, we intercept the cancel event.
    useEffect(() => {
        const dialog = dialogRef.current;
        if (!dialog) return;

        const handleCancel = (event: Event) => {
            event.preventDefault();
        };

        dialog.addEventListener('cancel', handleCancel);

        // Cleanup the listener when component unmounts
        return () => {
            dialog.removeEventListener('cancel', handleCancel);
        };
    }, []);

    // 3. Pagination Logic
    const currentPage = howToPlayPages[currentPageIndex];
    const isFirstPage = currentPageIndex === 0;
    const isLastPage = currentPageIndex === howToPlayPages.length - 1;

    const navigateNext = () => {
        if (!isLastPage) {
            setCurrentPageIndex(prev => prev + 1);
        }
    };

    const navigatePrev = () => {
        if (!isFirstPage) {
            setCurrentPageIndex(prev => prev - 1);
        }
    };

    return (
        <dialog
            ref={dialogRef}
            // Tailwind styling for the modal container itself
            className="fixed inset-0 m-auto
            w-[90%] max-w-2xl max-h-[90vh]
            rounded-lg border border-slate-700 bg-slate-900 p-0 shadow-2xl focus:outline-none
            backdrop:bg-black/70 backdrop:backdrop-blur-sm
            "
        >
            {/* Inner Content Wrapper (for padding and structure) */}
            <div className="flex flex-col h-full">
                
                {/* --- HEADER --- */}
                <div className="flex items-center justify-between border-b border-slate-700 p-6">
                    <h2 className="text-2xl font-extrabold text-white tracking-tight">
                        How to Play
                    </h2>
                    
                    {/* Close Button */}
                    <button
                        onClick={onClose}
                        className="rounded-full p-2 text-slate-500 hover:bg-slate-800 hover:text-white transition-colors focus:ring-2 focus:ring-green-500 focus:outline-none"
                        aria-label="Close Help"
                    >
                        <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                {/* --- PAGE CONTENT --- (Scrollable) */}
                <div className="flex-1 p-6 overflow-y-auto">
                    <h1 className="text-3xl font-black text-green-400 mb-6">
                        {currentPage.title}
                    </h1>
                    
                    <HelpPageRenderer items={currentPage.items} />
                </div>

                {/* --- FOOTER (Navigation) --- */}
                <div className="flex items-center justify-between border-t border-slate-700 p-6 bg-slate-950 rounded-b-lg">
                    {/* Previous Button (Left Arrow) */}
                    <button
                        onClick={navigatePrev}
                        disabled={isFirstPage}
                        className={`flex items-center gap-2 px-4 py-2 rounded font-medium transition-colors ${
                            isFirstPage
                                ? 'text-slate-700 cursor-not-allowed'
                                : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                        }`}
                    >
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                        </svg>
                        Previous
                    </button>
                    
                    {/* Page Indicator */}
                    <span className="text-sm font-mono text-slate-600">
                        {currentPageIndex + 1} / {howToPlayPages.length}
                    </span>

                    {/* Next Button (Right Arrow) */}
                    <button
                        onClick={navigateNext}
                        disabled={isLastPage}
                        className={`flex items-center gap-2 px-4 py-2 rounded font-medium transition-colors ${
                            isLastPage
                                ? 'text-slate-700 cursor-not-allowed'
                                : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                        }`}
                    >
                        Next
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                        </svg>
                    </button>
                </div>
            </div>
        </dialog>
    );
};