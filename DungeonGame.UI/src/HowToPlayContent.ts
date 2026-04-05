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
            { type: 'text', content: "You spend attack energy equal to the monster's defence stat to deal 1 damage." },
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
    },
    {
        id: 4,
        title: "Monster Movement",
        items: [
            { type: 'text', content: "You don't need to know how monsters behave to play the game but if you really want to know..." },
            { type: 'text', content: "Monsters move one at a time, in order of distance from you, starting with the closest." },
            { type: 'text', content: "They will move towards the tile at their maximum range from you with line of sight. If there are no tiles in range and in line of sight, they will move as close as possible to you." },
            { type: 'text', content: "If there are multiple options, they will prioritise the closest, then if still tied the leftmost tile, then the bottommost tile." },
            { type: 'image', content: 'howtoplay/monstermove-1.png' },
            { type: 'text', content: "In the above example, Ty moves to be at range 3 because Spiders have 3 attack range, while Ruby moves to be at range 3 top-right of the hero since it requires the least movement compared to top-left of the hero." }
        ]
    },
    {
        id: 5,
        title: "Monster Attack",
        items: [
            { type: 'text', content: "After all monsters have moved, monsters with enough attack range and line of sight of you, attack." },
            { type: 'image', content: 'howtoplay/monsterattack-1.png' },
            { type: 'text', content: "Damage taken = (sum of attacking monster's attack) / (your defence energy), rounded down." },
        ]
    },
    {
        id: 6,
        title: "Level end",
        items: [
            { type: 'text', content: "The level ends after you have defeated all monsters." },
            { type: 'image', content: 'howtoplay/levelend-1.png' },
            { type: 'text', content: "Upgrade a base stat or replenish your health to full as a reward before proceeding to the next level." },
        ]
    }
];