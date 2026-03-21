using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DungeonGame.Engine;
using DungeonGame.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<IGameEngine, GameEngine>();

var host = builder.Build();

var engine = host.Services.GetRequiredService<IGameEngine>();
JSBridge.Initialize(engine);

await host.RunAsync();
