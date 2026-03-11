using ClubBaist.Data;
using ClubBaist.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubBaist.Services;

public class TeeTimeService
{
    private readonly IDbContextFactory<ClubBaistDbContext> _dbFactory;

    // season changed to March bc i need to test this webapp
    // was April before, prolly should match the actual club rules eventually
    public static readonly DateOnly SeasonStart = new DateOnly(DateTime.Now.Year, 3, 1);
    public static readonly DateOnly SeasonEnd = new DateOnly(DateTime.Now.Year, 9, 30);

    // 8-min intervals from 7am to 6:30pm
    private static readonly TimeOnly DayStart = new TimeOnly(7, 0);
    private static readonly TimeOnly DayEnd   = new TimeOnly(18, 30);
    private const int IntervalMinutes = 8;

    public TeeTimeService(IDbContextFactory<ClubBaistDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public bool IsInSeason(DateOnly date) => date.Month >= 3 && date.Month <= 9;

    public bool IsWeekendOrHoliday(DateOnly date) =>
        date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

    // gold gets anytime, silver/bronze have restricted windows on weekdays and weekends
    // copper isnt in here at all 
    public bool IsTimeAllowedForTier(MembershipTier tier, DateOnly date, TimeOnly time)
    {
        bool isWeekend = IsWeekendOrHoliday(date);

        return tier switch
        {
            MembershipTier.Gold => true,
            MembershipTier.Silver => isWeekend
                ? time >= new TimeOnly(11, 0)
                : time < new TimeOnly(15, 0) || time >= new TimeOnly(17, 30),
            MembershipTier.Bronze => isWeekend
                ? time >= new TimeOnly(13, 0)
                : time < new TimeOnly(15, 0) || time >= new TimeOnly(18, 0),
            _ => false
        };
    }

    public List<TimeOnly> GetAllSlots()
    {
        var slots = new List<TimeOnly>();
        TimeOnly current = DayStart;
        while (current <= DayEnd)
        {
            slots.Add(current);
            current = current.AddMinutes(IntervalMinutes);
        }
        return slots;
    }

    public async Task<List<TimeOnly>> GetAvailableSlotsAsync(DateOnly date, MembershipTier tier)
    {
        await using var db = _dbFactory.CreateDbContext();
        var bookedTimes = (await db.TeeTimeBookings
            .Where(b => b.TeeDate == date && b.Status == BookingStatus.Confirmed)
            .Select(b => b.TeeTime)
            .ToListAsync()).ToHashSet();

        return GetAllSlots()
            .Where(t => IsTimeAllowedForTier(tier, date, t) && !bookedTimes.Contains(t))
            .ToList();
    }

    public async Task<List<TeeTimeBooking>> GetBookingsForDateAsync(DateOnly date)
    {
        await using var db = _dbFactory.CreateDbContext();
        return await db.TeeTimeBookings
            .Include(b => b.MemberProfile)
            .Where(b => b.TeeDate == date && b.Status != BookingStatus.Cancelled)
            .OrderBy(b => b.TeeTime)
            .ToListAsync();
    }

    public async Task<List<TeeTimeBooking>> GetMemberUpcomingBookingsAsync(int memberProfileId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        await using var db = _dbFactory.CreateDbContext();

        return await db.TeeTimeBookings
            .Where(b => b.MemberProfileId == memberProfileId
                        && b.TeeDate >= today
                        && b.Status == BookingStatus.Confirmed)
            .OrderBy(b => b.TeeDate)
            .ThenBy(b => b.TeeTime)
            .ToListAsync();
    }

    // returns null on success, error string on failure, caller checks for null to know if it worked
    public async Task<string?> BookTeeTimeAsync(
        int memberProfileId, MembershipTier tier,
        DateOnly date, TimeOnly time,
        int players, int carts,
        string? player2, string? player3, string? player4, string? notes)
    {
        // separate context here, need to check suspended status before the conflict check
        // easier to keep these apart than try to reuse one context across both checks
        await using var statusDb = _dbFactory.CreateDbContext();
        var member = await statusDb.MemberProfiles.FindAsync(memberProfileId);

        if (member != null && member.Status == MemberStatus.Suspended)
            return "Your membership is suspended. Please contact the pro shop.";

        if (!IsInSeason(date))
            return "Bookings are only available during golf season (March 1 \u2013 September 30).";

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (date < today)
            return "Cannot book a tee time in the past.";
        // TODO: maybe make advance days configurable instead of hardcoding 7
        if (date > today.AddDays(7))
            return "Tee times can only be booked up to 7 days in advance.";

        if (!IsTimeAllowedForTier(tier, date, time))
            return $"Your {tier} membership does not allow booking at {time:h\\:mm tt} on {(IsWeekendOrHoliday(date) ? "weekends" : "weekdays")}.";

        if (players < 1 || players > 4)
            return "Bookings must be for 1 to 4 players.";

        await using var db = _dbFactory.CreateDbContext();

        var conflict = await db.TeeTimeBookings.AnyAsync(b =>
            b.TeeDate == date && b.TeeTime == time && b.Status == BookingStatus.Confirmed);
        if (conflict)
            return "That tee time is already booked. Please choose another slot.";

        bool existingBooking = await db.TeeTimeBookings.AnyAsync(b =>
            b.MemberProfileId == memberProfileId
            && b.TeeDate == date
            && b.Status == BookingStatus.Confirmed);
        if (existingBooking)
            return "You already have a booking on that date.";

        db.TeeTimeBookings.Add(new TeeTimeBooking
        {
            MemberProfileId = memberProfileId,
            TeeDate         = date,
            TeeTime         = time,
            NumberOfPlayers = players,
            NumberOfCarts   = carts,
            Player2Name     = player2,
            Player3Name     = player3,
            Player4Name     = player4,
            Notes           = notes,
            Status          = BookingStatus.Confirmed,
            BookedAt        = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        return null;
    }

    // member can only cancel thier own bookings, ownership check is baked into the query already
    public async Task<bool> CancelBookingAsync(int bookingId, int memberProfileId)
    {
        await using var db = _dbFactory.CreateDbContext();
        var booking = await db.TeeTimeBookings
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.MemberProfileId == memberProfileId);

        if (booking == null || booking.Status != BookingStatus.Confirmed) return false;

        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync();
        return true;
    }

    // admin version skips the ownership check entirely, can nuke any booking like a city boi
    public async Task<bool> AdminCancelBookingAsync(int bookingId)
    {
        await using var db = _dbFactory.CreateDbContext();
        var booking = await db.TeeTimeBookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking == null || booking.Status != BookingStatus.Confirmed) return false;

        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync();
        return true;
    }
}
