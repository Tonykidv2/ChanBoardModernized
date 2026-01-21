using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.API.EndPoints;
using ChanBoardModernized.API.EndPointsl;
using ChanBoardModernized.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<CommentCounterService>();
builder.Services.AddTransient<AuthService>();

builder.Services.AddDbContext<ChanContext>(options =>
{
    var ctString = builder.Configuration.GetConnectionString("ChanBoardMongoDB") ?? throw new InvalidOperationException("Connection string 'ChanBoardMongoDB' not found.");
    var dbName = builder.Configuration.GetValue<string>("DatabaseName") ?? throw new InvalidOperationException("Database name not configured.");
    options.UseMongoDB(ctString, dbName);
    //options.UseInMemoryDatabase("ChanBoardInMemoryDB");
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

app.UseHttpsRedirection();

app.UseAuthentication(); //Call this first!!!
app.UseAuthorization(); //Then this for auth

app.MapControllers();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapChanBoardEndPoints();

app.MapGet("/", () => "Welcome to ChanBoardModernized API!");

app.MapGet("/health", () => Results.Ok("API is running!"));

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChanContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    await DbInitializer.SeedAsync(context, hasher);
}

app.Run();
