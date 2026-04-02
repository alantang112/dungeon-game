import { assetPath } from "../constants/assetConstants";
import { TileType } from "../models/TileType";

export const AssetPreloader = () => (
  <div className="hidden" aria-hidden="true">
    {([1, 2, 3, 4, 5, 6].map(number => <img src={`${assetPath}dice${number}.png`} />))}
    {(Object.values(TileType).filter(type => type != "Empty").map(type => <img src={`${assetPath}${type.toLowerCase()}.png`} />))}
  </div>
);