import { assetPath, d4FileName, diceFileName } from "../constants/assetConstants";
import { TileType } from "../models/TileType";

export const AssetPreloader = () => (
  <div className="hidden" aria-hidden="true">
    {([1, 2, 3, 4, 5, 6].map(number => <div key={`d6-asset-${number}`}><img src={`${assetPath}${diceFileName}${number}.png`} /></div>))}
    {([1, 2, 3, 4].map(number => <div key={`d4-asset-${number}`}><img src={`${assetPath}${d4FileName}${number}.png`} /></div>))}
    {(Object.values(TileType).filter(type => type != "Empty").map((type, i) => <div key={`tile-asset-${i}`}><img src={`${assetPath}${type.toLowerCase()}.png`} /></div>))}
  </div>
);