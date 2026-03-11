using ClubBaist.Data;
using ClubBaist.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubBaist.Services;

public class MemberService
{
    private readonly ClubBaistDbContext _db;

    // forgot waht i commented here TODO: remember
    // static so you can call GetAnnualFee without needing a db instance
    public static readonly Dictionary<(MembershipTier, MembershipType), decimal> AnnualFees = new()
    {
        [(MembershipTier.Gold,   MembershipType.Shareholder)]       = 3000m,
        [(MembershipTier.Gold,   MembershipType.Associate)]         = 4500m,
        [(MembershipTier.Silver, MembershipType.ShareholderSpouse)] = 2000m,
        [(MembershipTier.Silver, MembershipType.AssociateSpouse)]   = 2500m,
        [(MembershipTier.Bronze, MembershipType.PeeWee)]            = 250m,
        [(MembershipTier.Bronze, MembershipType.Junior)]            = 500m,
        [(MembershipTier.Bronze, MembershipType.Intermediate)]      = 1000m,
        [(MembershipTier.Copper, MembershipType.Social)]            = 100m,
    };

    public const decimal SharePurchasePrice = 1000m;
    public const decimal EntranceFee = 10000m;
    public const decimal MinFoodBeverageGold = 500m;
    public const decimal LatePaymentPenalty = 0.10m;

    public MemberService(ClubBaistDbContext db)
    {
        _db = db;
    }

    public async Task<MemberProfile?> GetByUserIdAsync(int userId)
    {
        return await _db.MemberProfiles
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId);
    }

    public async Task<MemberProfile?> GetByMemberNumberAsync(string memberNumber)
    {
        if (memberNumber == null) return null;

        return await _db.MemberProfiles
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberNumber == memberNumber);
    }

    public async Task<List<MemberProfile>> GetAllMembersAsync()
    {
        return await _db.MemberProfiles
            .Include(m => m.User)
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToListAsync();
    }

    // format is CB-YYYY-NNNN e.g. CB-2024-0001, count based so not truly unique if you delete records but good enough for now
    public async Task<string> GenerateMemberNumberAsync()
    {
        int year = DateTime.Now.Year;
        var count = await _db.MemberProfiles.CountAsync() + 1;
        return "CB-" + year.ToString() + "-" + count.ToString("D4");
    }

    // always starts as Active, admin can suspend later if needed.
    //TODO: add auditing for staff
    public async Task<MemberProfile> CreateMemberAsync(
        int userId,
        string firstName,
        string lastName,
        MembershipTier tier,
        MembershipType memberType,
        string? phone = null,
        DateOnly? dateOfBirth = null)
    {
        var memberNumber = await GenerateMemberNumberAsync();

        var profile = new MemberProfile
        {
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            MemberNumber = memberNumber,
            Tier = tier,
            MemberType = memberType,
            Status = MemberStatus.Active,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            MemberSince = DateTime.UtcNow
        };

        _db.MemberProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return profile;
    }

    public async Task UpdateHandicapAsync(int memberProfileId)
    {
        var member = await _db.MemberProfiles.FindAsync(memberProfileId);
        if (member == null) return;

        var rounds = await _db.PlayerRounds
            .Where(r => r.MemberProfileId == memberProfileId && r.IsApprovedCourse)
            .ToListAsync();

        member.HandicapIndex = HandicapService.CalculateHandicapIndex(rounds);
        await _db.SaveChangesAsync();
    }

    // static bc you dont need a db call just to look up a fee 
    public static decimal GetAnnualFee(MembershipTier tier, MembershipType type)
    {
        return AnnualFees.TryGetValue((tier, type), out var fee) ? fee : 0m;
    }
}
