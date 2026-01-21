using ChanBoardModernized.Shared.Components.Implementations;
using ChanBoardModernized.Shared.Components.Interfaces;
using ChanBoardModernized.Shared.Services;
using ChanBoardModernized.Web.Client.Services;
using Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the ChanBoardModernized.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<IAuthState, AuthState>();
builder.Services.AddSingleton<ITokenStore, WebTokenStore>();
//builder.Services.AddSingleton<HttpClient>(hp =>
//    new HttpClient
//    {
//        BaseAddress = new Uri("https://localhost:32001/")
//    });
builder.Services.AddScoped<TokenRefreshHandler>();
builder.Services.AddScoped<TokenRefreshService>();
builder.Services.AddHttpClient("ChanAPIClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:32001/");
}).AddHttpMessageHandler<TokenRefreshHandler>();
builder.Services.AddSingleton<IChanBoardHttpClient, ChanBoardHttpClient>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly, typeof(ChanBoardModernized.Shared.States.BoardState).Assembly);
});

await builder.Build().RunAsync();
