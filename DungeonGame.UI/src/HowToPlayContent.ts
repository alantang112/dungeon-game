export interface HelpItem {
    type: 'heading' | 'text' | 'image' | 'separator';
    content: string; // The text content or the image src URL
    alt?: string;    // Optional alt text for images
}

export interface HelpPage {
    id: number;
    title: string;
    items: HelpItem[];
}

export const howToPlayPages: HelpPage[] = [
    {
        id: 1,
        title: "Dungeon Game",
        items: [
            { type: 'text', content: "This is a turn based action game played on a 5x5 grid." },
            { type: 'text', content: 'Each turn consists of 1) rolling energy dice, 2) your actions (moving and attacking), and 3) monster actions (moving and attacking).'},
            { type: 'image', content: '/howtoplay/overview-1.png'},
        ]
    },
    {
        id: 2,
        title: "Energy dice",
        items: [
            { type: 'text', content: "Each round, you roll 3 dice and assign them to Movement, Attack or Defence (one die per stat)." },
            { type: 'image', content: '/howtoplay/energydice-1.png'},
            { type: 'text', content: "Energy points = Base stat + Energy dice."},
            { type: 'image', content: '/howtoplay/stats-1.png'},
        ]
    },
    {
        id: 3,
        title: "Moving Your Hero",
        items: [
            { type: 'text', content: "Click on any tile adjacent to your hero to move." },
            { type: 'image', content: '/howtoplay/movement-1.png' },
            { type: 'text', content: "It costs 2 movement points to move orthogonally and 3 movement points to move diagonally." },
            { type: 'text', content: "You can't hop diagonally through walls." },
            { type: 'text', content: "You can hop diagonally past monsters." },
        ]
    },
    {
        id: 4,
        title: "Hero Combat",
        items: [
            { type: 'text', content: "Click on monster tiles to attack. They must be in range and in line of sight." },
            { type: 'heading', content: "Range" },
            { type: 'text', content: "Range is determined the same way as movement; attacking an orthogonal tile requires 2 attack range." },
            { type: 'image', content: 'howtoplay/attack-1.png' }
        ]
    },
    {
        id: 4,
        title: "Hero Combat II",
        items: [
            { type: 'heading', content: "Line of sight" },
            { type: 'text', content: "You have line of sight if you can draw an uninterrupted line between any corner of your tile to the target tile." },
            { type: 'text', content: "You don't have line of sight through walls and monsters." },
            { type: 'image', content: 'howtoplay/lineofsight-1.png' },
            { type: 'image', content: 'howtoplay/lineofsight-2.png' }
        ]
    }
];