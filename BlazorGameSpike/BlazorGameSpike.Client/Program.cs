using BlazorGameSpike.Client.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<PersistedGameState>();
builder.Services.AddScoped<GameState, PersistedGameState>(sp => sp.GetRequiredService<PersistedGameState>());

var app = builder.Build();

// Load game save before start, so its available in teh DI
var persistedGameState = app.Services.GetRequiredService<PersistedGameState>();
await persistedGameState.LoadAsync();

await app.RunAsync();