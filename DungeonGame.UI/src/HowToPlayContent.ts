import { assetPath, howToPlayAssetsPath } from "./constants/assetConstants";

export interface HelpItem {
    type: 'heading' | 'text' | 'image' | 'separator' | 'ordered-list' | 'unordered-list';
    content?: string; // The text content or the image src URL
    contents?: string[]; // The text content for ordered list
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
            { type: 'text', content: 'Each turn consists of:'},
            { type: 'ordered-list', contents: ['Rolling energy dice', 'Hero actions (moving and attack)', 'Monsters movement', 'Monsters attack'] },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}overview-1.png`},
        ]
    },
    {
        id: 2,
        title: "Energy dice",
        items: [
            { type: 'text', content: "Each round, you roll 3 dice and assign them to Movement, Attack or Defence (one die per stat)." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}energydice-1.png`},
            { type: 'text', content: "Energy points = Base stat + Energy dice."},
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}stats-1.png`},
            { type: 'text', content: 'You are allowed one re-roll once per level.' },
        ]
    },
    {
        id: 3,
        title: "Hero movement",
        items: [
            { type: 'text', content: "Click on any tile adjacent to your hero to move." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}movement-1.png` },
            { type: 'text', content: "Movement energy requirements:" },
            { type: 'unordered-list', contents: ["2 energy to move orthogonally", "3 energy to move diagonally."] },
            { type: 'text', content: "You can't hop diagonally through walls." },
            { type: 'text', content: "You can hop diagonally past monsters." },
        ]
    },
    {
        id: 4,
        title: "Hero combat",
        items: [
            { type: 'text', content: "Click on monster tiles to attack. They must be in range and in line of sight." },
            { type: 'text', content: "You spend attack energy equal to the monster's defence stat to deal 1 damage." },
            { type: 'heading', content: "Range" },
            { type: 'text', content: "Range is determined the same way as movement; attacking an orthogonal tile requires 2 attack range." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}attack-1.png` }
        ]
    },
    {
        id: 4,
        title: "Hero combat II",
        items: [
            { type: 'heading', content: "Line of sight" },
            { type: 'text', content: "You have line of sight if you can draw an uninterrupted line between any corner of your tile to the target tile." },
            { type: 'text', content: "You don't have line of sight through walls and monsters." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}lineofsight-1.png` },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}lineofsight-2.png` }
        ]
    },
    {
        id: 5,
        title: "Hero combat III",
        items: [
            { type: 'heading', content: "Line of sight continued" },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}lineofsight-3.png` },
            { type: 'text', content: "There is line of sight with Wanda but not with Marge since Wanda blocks line of sight. Marge also does not have line of sight of you." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}lineofsight-4.png` },
            { type: 'text', content: "There is line of sight with Wanda but not with Marge since Wanda and the wall blocks line of sight. Marge also does not have line of sight of you." }
        ]
    },
    {
        id: 6,
        title: "Monster movement",
        items: [
            { type: 'text', content: "You don't need to know how monsters behave to play the game but it is required to master the game." },
            { type: 'text', content: "Monsters move one at a time, in order of distance from you, starting with the closest." },
            { type: 'text', content: "They will move towards the tile at their maximum range from you with line of sight. If there are no tiles in range and in line of sight, they will move as close as possible to you." },
            { type: 'text', content: "If there are multiple options, they will prioritise the closest, then if still tied the leftmost tile, then the bottommost tile." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}monstermove-1.png` },
            { type: 'text', content: "In the above example, Ty moves to be at range 3 because Spiders have 3 attack range, while Ruby moves to be at range 3 top-right of the hero since it requires the least movement compared to top-left of the hero." }
        ]
    },
    {
        id: 7,
        title: "Monster combat",
        items: [
            { type: 'text', content: "After all monsters have moved, monsters that can reach you with their attack range and that has line of sight of you, attack." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}monsterattack-1.png` },
            { type: 'text', content: "Damage taken = (sum of attacking monster's attack stat) / (your defence energy), rounded down." },
        ]
    },
    {
        id: 8,
        title: "Level end",
        items: [
            { type: 'text', content: "The level ends when you have defeated all monsters." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}levelend-1-v2.png` },
            { type: 'text', content: "At the start of the next level, you can permanently increase a base stat by 1 or replenish your health to full." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}levelend-2.png` },
        ]
    },
    {
        id: 9,
        title: "Game end",
        items: [
            { type: 'text', content: "If your health would drop to zero, the game ends for you." },
            { type: 'text', content: "You start every game with three lives which allows you to reattempt the current level and also reselect which upgrade to take, if there was one. Lives are represented by the orange pips." },
            { type: 'image', content: `${assetPath}${howToPlayAssetsPath}gamend-1.png` },
            { type: 'text', content: "The final level is level 16." },
            { type: 'text', content: "Happy dungeoning." },
        ]
    }
];