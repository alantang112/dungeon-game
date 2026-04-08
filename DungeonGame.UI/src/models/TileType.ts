export const TileType = {
  Empty: "Empty",
  Hero: "Hero",
  HeroRIP: "Hero-RIP",
  Wall: "Wall",
  NightmareWall: "Nightmare-Wall",
  Spider: "Spider",
  Skeleton: "Skeleton",
  Minotaur: "Minotaur",
  Fiendling: "Fiendling",
  Colossus: "Colossus",
  Overseer: "Overseer",
  Overseer2: "Overseer-2",
  Direwolf: "Direwolf",
  Direwolf2: "Direwolf-2",
  Direwolf3: "Direwolf-3",
  Reaper: "Reaper",
  Reaper2: "Reaper-2",
  Oathbound: "Oathbound",
  Oathbound2: "Oathbound-2",
  Elfling: "Elfling",
  Nightmare: "Nightmare",
  NightmareDamaged: "Nightmare-Damaged",
  NightmareBoss: "Nightmare-Boss",
  NightmareBossDamaged: "Nightmare-Boss-Damaged"
} as const;

export type TileType = typeof TileType[keyof typeof TileType];