export const TileType = {
  Empty: "Empty",
  Hero: "Hero",
  Wall: "Wall",
  Spider: "Spider",
  Skeleton: "Skeleton",
  Minotaur: "Minotaur",
  Hellspawn: "Hellspawn",
  Colossus: "Colossus"
} as const;

export type TileType = typeof TileType[keyof typeof TileType];