import { assetPath, d4FileName, diceFileName } from "../constants/assetConstants";
import { TileType } from "../models/TileType";

export const AssetPreloader = () => (
  <div className="hidden" aria-hidden="true">
    {([1, 2, 3, 4, 5, 6].map(number => <img src={`${assetPath}${diceFileName}${number}.png`} />))}
    {([1, 2, 3, 4].map(number => <img src={`${assetPath}${d4FileName}${number}.png`} />))}
    {(Object.values(TileType).filter(type => type != "Empty").map(type => <img src={`${assetPath}${type.toLowerCase()}.png`} />))}
  </div>
);