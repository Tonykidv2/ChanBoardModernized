using ChanBoardModernized.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.Data;

public class ChanContext : DbContext
{
    public ChanContext(DbContextOptions<ChanContext> options, IPasswordHasher<User> passwordHasher) : base(options)
    {
        PasswordHasher = passwordHasher;
    }

    public DbSet<Board> Boards { get; set; } = null!;
    public DbSet<Entities.Thread> Threads { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Photo> Photos { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<CommentCounter> CommentCounters { get; set; } = null!;
    public IPasswordHasher<User> PasswordHasher { get; }

    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
