namespace ClubBaist.Models;

public class StandingTeeTimeRequest
{
    public int RequestId { get; set; }

    // Primary member must be a gold shareholder
    public int PrimaryMemberProfileId { get; set; }
    public MemberProfile PrimaryMemberProfile { get; set; } = null!;

    // Foursome only, up to 3 additional members
    public string? Member2Number { get; set; }
    public string? Member2Name { get; set; }
    public string? Member3Number { get; set; }
    public string? Member3Name { get; set; }
    public string? Member4Number { get; set; }
    public string? Member4Name { get; set; }

    public DayOfWeek RequestedDayOfWeek { get; set; }
    public TimeOnly RequestedTime { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Assigned by the committee
    public int? PriorityNumber { get; set; }
    public TimeOnly? ApprovedTime { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }

    public bool IsActive { get; set; } = true;
}
