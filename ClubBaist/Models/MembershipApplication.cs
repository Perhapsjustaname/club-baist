namespace ClubBaist.Models;

public class MembershipApplication
{
    public int ApplicationId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? AlternatePhone { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? Occupation { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyPostalCode { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public MembershipType RequestedType { get; set; } = MembershipType.Associate;

    // Sponsors, must be Gold Shareholders with 5+ years, can sponsor up to 2per year
    public string? Sponsor1MemberNumber { get; set; }
    public string? Sponsor1Name { get; set; }
    public string? Sponsor1Signature { get; set; }
    public DateTime? Sponsor1Date { get; set; }

    public string? Sponsor2MemberNumber { get; set; }
    public string? Sponsor2Name { get; set; }
    public string? Sponsor2Signature { get; set; }
    public DateTime? Sponsor2Date { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? ReviewNotes { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public enum ApplicationStatus { Pending, UnderReview, Accepted, Denied, OnHold, Waitlisted }
