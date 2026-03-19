using ChanBoardModernized.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.Data;

public class ChanContext : DbContext
{
    public ChanContext(DbContextOptions<ChanContext> options, IPasswordHasher<User> passwordHasher, IConfiguration configuration, ILogger<ChanContext> logger) : base(options)
    {
        PasswordHasher = passwordHasher;
        Logger = logger;

        //check if we are on Pi and if so, disable transactions for better performance
        var deploymentTarget = configuration.GetValue<string>("DEPLOYMENT_TARGET") ?? "server";
        Logger.LogInformation("Deployment target: {DeploymentTarget}", deploymentTarget);
        //database type
        Logger.LogInformation("Database provider: {DatabaseProvider}", Database.ProviderName);
        if (deploymentTarget.Equals("pi", StringComparison.OrdinalIgnoreCase))
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }
    }

    public DbSet<Board> Boards { get; set; } = null!;
    public DbSet<Entities.Thread> Threads { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Photo> Photos { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<CommentCounter> CommentCounters { get; set; } = null!;
    public IPasswordHasher<User> PasswordHasher { get; }
    public ILogger<ChanContext> Logger { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Board configuration
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShortName).IsUnique();
        });

        // Thread configuration
        modelBuilder.Entity<Entities.Thread>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(t => t.Board)
                .WithMany()
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.BoardId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Comment configuration
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(c => c.Thread)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.CommentPhoto)
                .WithMany()
                .HasForeignKey(c => c.CommentPhotoId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.ThreadId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Photo configuration
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);
        });

        // CommentCounter configuration
        modelBuilder.Entity<CommentCounter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BoardId).IsUnique();
        });
    }
}
