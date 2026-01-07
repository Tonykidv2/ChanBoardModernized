using ChanBoardModernized.Shared.Components.Implementations;
using ChanBoardModernized.Shared.Components.Interfaces;
using ChanBoardModernized.Shared.Services;
using ChanBoardModernized.Web.Components;
using ChanBoardModernized.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the ChanBoardModernized.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<IAuthState, AuthState>();
builder.Services.AddSingleton<ITokenStore, WebTokenStore>();
builder.Services.AddSingleton<HttpClient>(hp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://host.docker.internal:32001/")
    });
builder.Services.AddHttpClient("ChanAPIClient", client =>
{
    client.BaseAddress = new Uri("https://host.docker.internal:32001/");
});
builder.Services.AddSingleton<IChanBoardHttpClient, ChanBoardHttpClient>();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(ChanBoardModernized.Shared._Imports).Assembly,
        typeof(ChanBoardModernized.Web.Client._Imports).Assembly);

app.Run();
