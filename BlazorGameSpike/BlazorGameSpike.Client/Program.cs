using BlazorGameSpike.Client.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<GameState>();

await builder.Build().RunAsync();
