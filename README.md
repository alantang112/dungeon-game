# Building and running locally
```
dotnet publish .\DungeonGame.Wasm\DungeonGame.Wasm.csproj -c Release
npm run --prefix DungeonGame.UI build
npm run --prefix DungeonGame.UI dev
```

# Debugging iPhone
```
npx playwright open --device="iPhone 13" http://localhost:5173
```
