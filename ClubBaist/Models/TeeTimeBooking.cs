namespace ClubBaist.Models;

public class TeeTimeBooking
{
    public int BookingId { get; set; }
    public int MemberProfileId { get; set; }
    public MemberProfile MemberProfile { get; set; } = null!;

    public DateOnly TeeDate { get; set; }
    public TimeOnly TeeTime { get; set; }
    public int NumberOfPlayers { get; set; } = 1;
    public int NumberOfCarts { get; set; } = 0;
    public string? Notes { get; set; }

    // Additional players in the group
    public string? Player2Name { get; set; }
    public string? Player3Name { get; set; }
    public string? Player4Name { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public string? BookedByStaff { get; set; }

    public bool IsStanding { get; set; } = false;

    public bool CheckedIn { get; set; } = false;
    public DateTime? CheckedInAt { get; set; }
}

public enum BookingStatus { Confirmed, Cancelled, NoShow, Completed }
