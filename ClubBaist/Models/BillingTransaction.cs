namespace ClubBaist.Models;

public class BillingTransaction
{
    public int TransactionId { get; set; }
    public int MemberProfileId { get; set; }
    public MemberProfile MemberProfile { get; set; } = null!;

    public BillingTransactionType Type { get; set; }

    // positive = charge (member owes us money), negative = payment or credit
    // balance = sum of all amounts → positive means member is in arrears
    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // billing year — annual fees are per season, F&B minimum resets each year
    public int Season { get; set; } = DateTime.UtcNow.Year;

    // who posted this — "system" for auto-charges, username for manual entries
    public string RecordedBy { get; set; } = "system";
}

public enum BillingTransactionType
{
    AnnualFee,        // yearly dues charge
    EntranceFee,      // one-time entrance fee charge
    FoodBeverageCharge, // F&B spend logged against the Gold member minimum
    Payment,          // money received from member
    Adjustment        // admin credit or debit (e.g. corrections, waivers)
}
