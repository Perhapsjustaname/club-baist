using Microsoft.EntityFrameworkCore;
using ClubBaist.Models;

namespace ClubBaist.Data;

public class ClubBaistDbContext : DbContext
{
    public ClubBaistDbContext(DbContextOptions<ClubBaistDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}