using ClubBaist.Data;
using ClubBaist.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubBaist.Services;

public class BillingService
{
    private readonly IDbContextFactory<ClubBaistDbContext> _dbFactory;

    // F&B minimum only applies to Gold Shareholders — $500/year
    public const decimal FoodBeverageMinimum = 500m;

    public BillingService(IDbContextFactory<ClubBaistDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // ── Annual fee lookup ─────────────────────────────────────────────────────

    // returns the annual due amount for a given membership type
    // matches the fee table in the README
    public static decimal GetAnnualFee(MembershipType type) => type switch
    {
        MembershipType.Shareholder       => 3000m,
        MembershipType.Associate         => 4500m,
        MembershipType.ShareholderSpouse => 2000m,
        MembershipType.AssociateSpouse   => 2500m,
        MembershipType.Intermediate      => 1000m,
        MembershipType.Junior            => 500m,
        MembershipType.PeeWee            => 250m,
        MembershipType.Social            => 100m,
        _                                => 0m
    };

    // ── Balance summary ───────────────────────────────────────────────────────

    // a flat summary of a members billing state — used in both member and admin views
    public async Task<MemberBillingSummary> GetSummaryAsync(int memberProfileId, int season)
    {
        await using var db = _dbFactory.CreateDbContext();

        var profile = await db.MemberProfiles
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberProfileId == memberProfileId);

        if (profile == null) return new MemberBillingSummary();

        var transactions = await db.BillingTransactions
            .Where(t => t.MemberProfileId == memberProfileId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var seasonTransactions = transactions.Where(t => t.Season == season).ToList();

        decimal totalCharged = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        decimal totalPaid    = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
        decimal balance      = totalCharged - totalPaid;

        // F&B tracking — sum all F&B charges for the current season
        decimal fbSpent = seasonTransactions
            .Where(t => t.Type == BillingTransactionType.FoodBeverageCharge && t.Amount > 0)
            .Sum(t => t.Amount);

        // check if annual fee was already posted for this season
        bool annualFeePosted = seasonTransactions
            .Any(t => t.Type == BillingTransactionType.AnnualFee);

        return new MemberBillingSummary
        {
            MemberProfileId  = memberProfileId,
            MemberNumber     = profile.MemberNumber,
            FullName         = profile.FullName,
            Tier             = profile.Tier,
            MemberType       = profile.MemberType,
            AnnualFee        = GetAnnualFee(profile.MemberType),
            TotalCharged     = totalCharged,
            TotalPaid        = totalPaid,
            Balance          = balance,
            FoodBeverageSpent     = fbSpent,
            FoodBeverageMinimum   = profile.IsGoldShareholder ? FoodBeverageMinimum : 0m,
            AnnualFeePostedThisSeason = annualFeePosted,
            Transactions     = transactions
        };
    }

    // all member summaries for the admin billing page — sorted by balance descending
    public async Task<List<MemberBillingSummary>> GetAllSummariesAsync(int season)
    {
        await using var db = _dbFactory.CreateDbContext();

        var profiles = await db.MemberProfiles
            .Include(m => m.User)
            .Where(m => m.Status == MemberStatus.Active || m.Status == MemberStatus.Suspended)
            .OrderBy(m => m.LastName)
            .ToListAsync();

        var allTransactions = await db.BillingTransactions.ToListAsync();

        var summaries = profiles.Select(profile =>
        {
            var txns         = allTransactions.Where(t => t.MemberProfileId == profile.MemberProfileId).ToList();
            var seasonTxns   = txns.Where(t => t.Season == season).ToList();
            decimal charged  = txns.Where(t => t.Amount > 0).Sum(t => t.Amount);
            decimal paid     = txns.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            decimal fbSpent  = seasonTxns.Where(t => t.Type == BillingTransactionType.FoodBeverageCharge && t.Amount > 0).Sum(t => t.Amount);
            bool feePosted   = seasonTxns.Any(t => t.Type == BillingTransactionType.AnnualFee);

            return new MemberBillingSummary
            {
                MemberProfileId           = profile.MemberProfileId,
                MemberNumber              = profile.MemberNumber,
                FullName                  = profile.FullName,
                Tier                      = profile.Tier,
                MemberType                = profile.MemberType,
                AnnualFee                 = GetAnnualFee(profile.MemberType),
                TotalCharged              = charged,
                TotalPaid                 = paid,
                Balance                   = charged - paid,
                FoodBeverageSpent         = fbSpent,
                FoodBeverageMinimum       = profile.IsGoldShareholder ? FoodBeverageMinimum : 0m,
                AnnualFeePostedThisSeason = feePosted,
                Transactions              = txns.OrderByDescending(t => t.TransactionDate).ToList()
            };
        }).ToList();

        return summaries;
    }

    // ── Transaction recording ─────────────────────────────────────────────────

    // post the annual dues for the given season — prevents double-posting for same year
    public async Task<(bool success, string message)> PostAnnualFeeAsync(
        int memberProfileId, int season, string recordedBy)
    {
        await using var db = _dbFactory.CreateDbContext();

        bool alreadyPosted = await db.BillingTransactions.AnyAsync(t =>
            t.MemberProfileId == memberProfileId &&
            t.Type == BillingTransactionType.AnnualFee &&
            t.Season == season);

        if (alreadyPosted)
            return (false, $"Annual fee for {season} has already been posted.");

        var profile = await db.MemberProfiles.FindAsync(memberProfileId);
        if (profile == null) return (false, "Member not found.");

        decimal fee = GetAnnualFee(profile.MemberType);

        db.BillingTransactions.Add(new BillingTransaction
        {
            MemberProfileId = memberProfileId,
            Type            = BillingTransactionType.AnnualFee,
            Amount          = fee,
            Description     = $"{season} Annual Dues – {profile.MemberType}",
            Season          = season,
            RecordedBy      = recordedBy
        });

        await db.SaveChangesAsync();
        return (true, $"Annual fee of ${fee:N0} posted for {season}.");
    }

    // record a payment received from the member (stored as negative — reduces balance)
    public async Task<(bool success, string message)> RecordPaymentAsync(
        int memberProfileId, decimal amount, string description, int season, string recordedBy)
    {
        if (amount <= 0) return (false, "Payment amount must be greater than zero.");

        await using var db = _dbFactory.CreateDbContext();

        db.BillingTransactions.Add(new BillingTransaction
        {
            MemberProfileId = memberProfileId,
            Type            = BillingTransactionType.Payment,
            Amount          = -amount, // negative = credit to the member's account
            Description     = string.IsNullOrWhiteSpace(description) ? "Payment received" : description,
            Season          = season,
            RecordedBy      = recordedBy
        });

        await db.SaveChangesAsync();
        return (true, $"Payment of ${amount:N2} recorded.");
    }

    // log a F&B charge for Gold Shareholders tracking towards the $500 minimum
    public async Task<(bool success, string message)> RecordFoodBeverageAsync(
        int memberProfileId, decimal amount, string description, int season, string recordedBy)
    {
        if (amount <= 0) return (false, "Amount must be greater than zero.");

        await using var db = _dbFactory.CreateDbContext();

        db.BillingTransactions.Add(new BillingTransaction
        {
            MemberProfileId = memberProfileId,
            Type            = BillingTransactionType.FoodBeverageCharge,
            Amount          = amount,
            Description     = string.IsNullOrWhiteSpace(description) ? "Food & Beverage charge" : description,
            Season          = season,
            RecordedBy      = recordedBy
        });

        await db.SaveChangesAsync();
        return (true, $"F&B charge of ${amount:N2} recorded.");
    }

    // post a manual adjustment — positive for extra charges, negative for credits/waivers
    public async Task<(bool success, string message)> RecordAdjustmentAsync(
        int memberProfileId, decimal amount, string description, int season, string recordedBy)
    {
        if (amount == 0) return (false, "Adjustment amount cannot be zero.");
        if (string.IsNullOrWhiteSpace(description)) return (false, "A description is required for adjustments.");

        await using var db = _dbFactory.CreateDbContext();

        db.BillingTransactions.Add(new BillingTransaction
        {
            MemberProfileId = memberProfileId,
            Type            = BillingTransactionType.Adjustment,
            Amount          = amount,
            Description     = description,
            Season          = season,
            RecordedBy      = recordedBy
        });

        await db.SaveChangesAsync();
        return (true, "Adjustment recorded.");
    }

    // post the one-time entrance fee charge
    public async Task<(bool success, string message)> PostEntranceFeeAsync(
        int memberProfileId, string recordedBy)
    {
        await using var db = _dbFactory.CreateDbContext();

        bool alreadyPosted = await db.BillingTransactions.AnyAsync(t =>
            t.MemberProfileId == memberProfileId &&
            t.Type == BillingTransactionType.EntranceFee);

        if (alreadyPosted)
            return (false, "Entrance fee has already been posted for this member.");

        int season = DateTime.UtcNow.Year;

        db.BillingTransactions.Add(new BillingTransaction
        {
            MemberProfileId = memberProfileId,
            Type            = BillingTransactionType.EntranceFee,
            Amount          = 10000m, // fixed $10,000 entrance fee per club rules
            Description     = "Entrance Fee",
            Season          = season,
            RecordedBy      = recordedBy
        });

        await db.SaveChangesAsync();
        return (true, "Entrance fee of $10,000 posted.");
    }
}

// ── Summary DTO ───────────────────────────────────────────────────────────────

// returned by GetSummaryAsync / GetAllSummariesAsync — no DB round trips needed after this
public class MemberBillingSummary
{
    public int MemberProfileId { get; set; }
    public string MemberNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public MembershipTier Tier { get; set; }
    public MembershipType MemberType { get; set; }

    public decimal AnnualFee { get; set; }         // what they should owe this year
    public decimal TotalCharged { get; set; }      // sum of all positive transactions
    public decimal TotalPaid { get; set; }         // sum of all payments/credits (positive value)
    public decimal Balance { get; set; }           // TotalCharged - TotalPaid → positive = owes money

    public decimal FoodBeverageSpent { get; set; }    // F&B logged this season
    public decimal FoodBeverageMinimum { get; set; }  // 500 for Gold Shareholders, 0 for everyone else
    public bool FoodBeverageMinimumMet => FoodBeverageMinimum == 0 || FoodBeverageSpent >= FoodBeverageMinimum;

    public bool AnnualFeePostedThisSeason { get; set; }
    public bool IsInArrears => Balance > 0;

    public List<BillingTransaction> Transactions { get; set; } = new();
}
