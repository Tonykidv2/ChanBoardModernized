using ChanBoardModernized.Shared.Components.Implementations;
using ChanBoardModernized.Shared.Components.Interfaces;
using ChanBoardModernized.Shared.Services;
using ChanBoardModernized.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the ChanBoardModernized.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<IAuthState, AuthState>();
builder.Services.AddSingleton<ITokenStore, WebTokenStore>();
builder.Services.AddSingleton<HttpClient>(hp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:32001/")
    });
builder.Services.AddHttpClient("ChanAPIClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:32001/");
});
builder.Services.AddSingleton<IChanBoardHttpClient, ChanBoardHttpClient>();


await builder.Build().RunAsync();
