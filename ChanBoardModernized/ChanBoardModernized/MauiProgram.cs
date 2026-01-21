using ChanBoardModernized.Services;
using ChanBoardModernized.Shared.Components.Implementations;
using ChanBoardModernized.Shared.Components.Interfaces;
using ChanBoardModernized.Shared.Services;
using Microsoft.Extensions.Logging;
using Fluxor;
using ChanBoardModernized.Shared;

namespace ChanBoardModernized
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the ChanBoardModernized.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<ITokenStore, SecureTokenStore>();
            builder.Services.AddSingleton<IAuthState, AuthState>();
            
            builder.Services.AddScoped<TokenRefreshHandler>();
            builder.Services.AddScoped<TokenRefreshService>();
            builder.Services.AddHttpClient("ChanAPIClient", client =>
            {
                client.BaseAddress = new Uri(GetApiBaseUrl());
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                #if DEBUG && ANDROID
                    return new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                #else
                    return new HttpClientHandler();
                #endif
            }).AddHttpMessageHandler<TokenRefreshHandler>();

            builder.Services.AddSingleton<IChanBoardHttpClient, ChanBoardHttpClient>();
            builder.Services.AddMauiBlazorWebView();
            
            builder.Services.AddBlazorBootstrap();
            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(MauiProgram).Assembly, typeof(Shared.States.BoardState).Assembly);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        static string GetApiBaseUrl()
        {
            #if ANDROID
                // Android emulator → host machine
                return "https://10.0.2.2:32001/";
            #elif IOS
                // iOS simulator → host machine
                return "https://localhost:32001/";
            #elif WINDOWS
                        return "https://localhost:32001/";
            #else
                return "https://localhost:32001/";
            #endif
        }
    }
}
