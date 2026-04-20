using Microsoft.EntityFrameworkCore;
using ClubBaist.Models;

namespace ClubBaist.Data;

public class ClubBaistDbContext : DbContext
{
    public ClubBaistDbContext(DbContextOptions<ClubBaistDbContext> options) : base(options) { }

    // these are the tables, EF maps each DbSet to a db table
    public DbSet<User> Users { get; set; }
    public DbSet<MemberProfile> MemberProfiles { get; set; }
    public DbSet<MembershipApplication> MembershipApplications { get; set; }
    public DbSet<TeeTimeBooking> TeeTimeBookings { get; set; }
    public DbSet<StandingTeeTimeRequest> StandingTeeTimeRequests { get; set; }
    public DbSet<PlayerRound> PlayerRounds { get; set; }
    public DbSet<HoleScore> HoleScores { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<BillingTransaction> BillingTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // unique indexes so two users cant have the same username or email
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // cascade so deleting a user also wipes thier profile, dont want orphaned records
        modelBuilder.Entity<MemberProfile>()
            .HasOne(m => m.User)
            .WithOne()
            .HasForeignKey<MemberProfile>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberProfile>().HasIndex(m => m.MemberNumber).IsUnique();

        // sql server needs explicit precision for decimals or it uses wei rddefaults
        modelBuilder.Entity<MemberProfile>()
            .Property(m => m.HandicapIndex).HasPrecision(5, 1);

        modelBuilder.Entity<MemberProfile>()
            .Property(m => m.EntranceFeeBalance).HasPrecision(10, 2);
        modelBuilder.Entity<MemberProfile>()
            .Property(m => m.AnnualFeeBalance).HasPrecision(10, 2);

        modelBuilder.Entity<MemberProfile>()
                .Property(m => m.FoodBeverageBalance).HasPrecision(10, 2);

        modelBuilder.Entity<TeeTimeBooking>()
            .HasOne(b => b.MemberProfile)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MemberProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerRound>()
            .HasOne(r => r.MemberProfile)
            .WithMany(m => m.Rounds)
            .HasForeignKey(r => r.MemberProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerRound>()
            .Property(r => r.CourseRating).HasPrecision(4, 1);

        modelBuilder.Entity<PlayerRound>()
            .Property(r => r.ScoreDifferential).HasPrecision(5, 1);

        modelBuilder.Entity<HoleScore>()
            .HasOne(h => h.Round)
            .WithMany(r => r.HoleScores)
            .HasForeignKey(h => h.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StandingTeeTimeRequest>()
            .HasOne(s => s.PrimaryMemberProfile)
            .WithMany(m => m.StandingRequests)
            .HasForeignKey(s => s.PrimaryMemberProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // EF 
        // the models use shorthand names so we have to tell EF explicitly
        modelBuilder.Entity<MembershipApplication>().HasKey(a => a.ApplicationId);
        modelBuilder.Entity<TeeTimeBooking>().HasKey(b => b.BookingId);
        modelBuilder.Entity<StandingTeeTimeRequest>().HasKey(s => s.RequestId);
        modelBuilder.Entity<PlayerRound>().HasKey(r => r.RoundId);
        modelBuilder.Entity<BillingTransaction>().HasKey(t => t.TransactionId);

        // BillingTransaction FK — cascade so a member delete also clears thier ledger
        modelBuilder.Entity<BillingTransaction>()
            .HasOne(t => t.MemberProfile)
            .WithMany()
            .HasForeignKey(t => t.MemberProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BillingTransaction>()
            .Property(t => t.Amount).HasPrecision(10, 2);

        // computed props dont have db columns, EF crashes if you dont Ignore
        //TODO NEEDS testing
        modelBuilder.Entity<MemberProfile>().Ignore(m => m.IsGoldShareholder);
        modelBuilder.Entity<MemberProfile>().Ignore(m => m.FullName);
        modelBuilder.Entity<MembershipApplication>().Ignore(a => a.FullName);
        modelBuilder.Entity<HoleScore>().Ignore(h => h.RelativeToPar);
    }
}
