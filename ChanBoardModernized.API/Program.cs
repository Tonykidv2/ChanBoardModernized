using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.API.EndPoints;
using ChanBoardModernized.API.EndPointsl;
using ChanBoardModernized.API.Middleware;
using ChanBoardModernized.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Detect deployment target from environment variable
var deploymentTarget = builder.Configuration.GetValue<string>("DEPLOYMENT_TARGET") ?? "server";
var isRaspberryPi = deploymentTarget.Equals("pi", StringComparison.OrdinalIgnoreCase);

// Configure Kestrel for Raspberry Pi wireless network access
if (isRaspberryPi)
{
    var port = builder.Configuration.GetValue<int>("PORT", 5000);
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddHostedService<RefreshTokenCleanupService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<CommentCounterService>();
builder.Services.AddTransient<AuthService>();

builder.Services.AddDbContext<ChanContext>(options =>
{
    var ctString = builder.Configuration.GetConnectionString("ChanBoardMongoDB") ?? throw new InvalidOperationException("Connection string 'ChanBoardMongoDB' not found.");
    var dbName = builder.Configuration.GetValue<string>("DatabaseName") ?? throw new InvalidOperationException("Database name not configured.");

    options.UseMongoDB(ctString, dbName);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("JWT:issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("JWT:audience"),
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:secret") ?? throw new InvalidOperationException("JWT secret not configured"))),

            RequireSignedTokens = true, // Reject unsigned tokens
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }, // Only accept HS256
            ValidTypes = new[] { "JWT" }
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Only use HTTPS redirection on server deployments (not Pi)
if (!isRaspberryPi)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication(); //Call this first!!!
app.UseRoleValidation();
app.UseAuthorization(); //Then this for auth

app.MapControllers();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapChanBoardEndPoints();

app.MapGet("/", () => new
{
    message = "Welcome to ChanBoardModernized API!",
    deployment = isRaspberryPi ? "Raspberry Pi" : "Server",
    version = "1.0.0"
});

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    deployment = isRaspberryPi ? "pi" : "server"
}));

//Hello world
app.MapGet("/hello", () => "Hello, world! This is the ChanBoardModernized API.");   

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChanContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    await DbInitializer.SeedAsync(context, hasher);
}

app.Run();
