namespace ClubBaist.Models;

public class MemberProfile
{
    public int MemberProfileId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;

    public MembershipTier Tier { get; set; } = MembershipTier.Bronze;
    public MembershipType MemberType { get; set; } = MembershipType.Junior;
    public MemberStatus Status { get; set; } = MemberStatus.Active;

    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Occupation { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyPostalCode { get; set; }

    public DateTime MemberSince { get; set; } = DateTime.UtcNow;
    public decimal? HandicapIndex { get; set; }

    // Financial tracking
    public decimal EntranceFeeBalance { get; set; } = 0;
    public decimal AnnualFeeBalance { get; set; } = 0;
    public decimal FoodBeverageBalance { get; set; } = 0;

    public bool IsGoldShareholder => Tier == MembershipTier.Gold && MemberType == MembershipType.Shareholder;
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<TeeTimeBooking> Bookings { get; set; } = new List<TeeTimeBooking>();
    public ICollection<PlayerRound> Rounds { get; set; } = new List<PlayerRound>();
    public ICollection<StandingTeeTimeRequest> StandingRequests { get; set; } = new List<StandingTeeTimeRequest>();
}

public enum MembershipTier { Gold, Silver, Bronze, Copper }

public enum MembershipType
{
    Shareholder,
    Associate,
    ShareholderSpouse,
    AssociateSpouse,
    PeeWee,
    Junior,
    Intermediate,
    Social
}

public enum MemberStatus { Active, Suspended, Inactive, Pending }
