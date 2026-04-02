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

    // ============================================================
    // STANDING TEE TIME REQUESTS
    // ============================================================

    // member submits a request - shareholder only, one per day of week
    public async Task<string?> SubmitStandingRequestAsync(
        int memberProfileId,
        DayOfWeek dayOfWeek, TimeOnly preferredTime,
        DateOnly startDate, DateOnly endDate,
        string? member2Number, string? member2Name,
        string? member3Number, string? member3Name,
        string? member4Number, string? member4Name)
    {
        await using var db = _dbFactory.CreateDbContext();
        var member = await db.MemberProfiles.FindAsync(memberProfileId);

        if (member == null) return "Member profile not found.";
        if (!member.IsGoldShareholder)
            return "Only Gold Shareholders may submit standing tee time requests.";

        if (!IsInSeason(startDate) || !IsInSeason(endDate))
            return "Start and end dates must be within the golf season (March 1 \u2013 September 30).";
        if (startDate > endDate)
            return "Start date must be before end date.";

        // dont allow duplicate pending requests for same day
        bool hasPending = await db.StandingTeeTimeRequests.AnyAsync(r =>
            r.PrimaryMemberProfileId == memberProfileId &&
            r.RequestedDayOfWeek == dayOfWeek &&
            r.Status == RequestStatus.Pending);
        if (hasPending)
            return "You already have a pending standing request for that day of the week.";

        db.StandingTeeTimeRequests.Add(new StandingTeeTimeRequest
        {
            PrimaryMemberProfileId = memberProfileId,
            RequestedDayOfWeek     = dayOfWeek,
            RequestedTime          = preferredTime,
            StartDate              = startDate,
            EndDate                = endDate,
            Member2Number          = member2Number,
            Member2Name            = member2Name,
            Member3Number          = member3Number,
            Member3Name            = member3Name,
            Member4Number          = member4Number,
            Member4Name            = member4Name,
            Status                 = RequestStatus.Pending,
            IsActive               = false
        });

        await db.SaveChangesAsync();
        return null; // null = success
    }

    public async Task<List<StandingTeeTimeRequest>> GetAllStandingRequestsAsync()
    {
        await using var db = _dbFactory.CreateDbContext();
        return await db.StandingTeeTimeRequests
            .Include(r => r.PrimaryMemberProfile)
            .OrderBy(r => r.Status)
            .ThenBy(r => r.RequestedDayOfWeek)
            .ThenBy(r => r.RequestedTime)
            .ToListAsync();
    }

    public async Task<List<StandingTeeTimeRequest>> GetMemberStandingRequestsAsync(int memberProfileId)
    {
        await using var db = _dbFactory.CreateDbContext();
        return await db.StandingTeeTimeRequests
            .Where(r => r.PrimaryMemberProfileId == memberProfileId)
            .OrderByDescending(r => r.RequestId)
            .ToListAsync();
    }

    // approves the request, assigns time + priority, and auto-books all matching dates in the season
    // returns success flag, message, and how many bookings were created
    public async Task<(bool success, string message, int bookedCount)> ApproveStandingRequestAsync(
        int requestId, TimeOnly approvedTime, int priority, string approvedBy)
    {
        await using var db = _dbFactory.CreateDbContext();
        var request = await db.StandingTeeTimeRequests
            .Include(r => r.PrimaryMemberProfile)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null) return (false, "Request not found.", 0);
        if (request.Status == RequestStatus.Approved) return (false, "Already approved.", 0);

        request.Status        = RequestStatus.Approved;
        request.ApprovedTime  = approvedTime;
        request.PriorityNumber = priority;
        request.ApprovedBy    = approvedBy;
        request.ApprovedDate  = DateTime.UtcNow;
        request.IsActive      = true;

        // walk every day in the date range, book on matching day of week
        var current = request.StartDate;
        int booked = 0;

        while (current <= request.EndDate)
        {
            if (current.DayOfWeek == request.RequestedDayOfWeek)
            {
                // skip if that slot is already taken by someone else
                bool conflict = await db.TeeTimeBookings.AnyAsync(b =>
                    b.TeeDate == current &&
                    b.TeeTime == approvedTime &&
                    b.Status  == BookingStatus.Confirmed);

                if (!conflict)
                {
                    db.TeeTimeBookings.Add(new TeeTimeBooking
                    {
                        MemberProfileId = request.PrimaryMemberProfileId,
                        TeeDate         = current,
                        TeeTime         = approvedTime,
                        NumberOfPlayers = CountStandingPlayers(request),
                        Player2Name     = request.Member2Name,
                        Player3Name     = request.Member3Name,
                        Player4Name     = request.Member4Name,
                        Status          = BookingStatus.Confirmed,
                        IsStanding      = true,
                        BookedAt        = DateTime.UtcNow,
                        BookedByStaff   = approvedBy
                    });
                    booked++;
                }
            }
            current = current.AddDays(1);
        }

        await db.SaveChangesAsync();
        return (true, $"Approved. {booked} tee time{(booked != 1 ? "s" : "")} booked for the season.", booked);
    }

    public async Task<bool> DeclineStandingRequestAsync(int requestId, string declinedBy, string? reason)
    {
        await using var db = _dbFactory.CreateDbContext();
        var request = await db.StandingTeeTimeRequests.FindAsync(requestId);
        if (request == null || request.Status != RequestStatus.Pending) return false;

        request.Status        = RequestStatus.Declined;
        request.DeclinedReason = reason;
        request.ApprovedBy    = declinedBy; // reusing field as "actioned by"
        request.ApprovedDate  = DateTime.UtcNow;
        request.IsActive      = false;

        await db.SaveChangesAsync();
        return true;
    }

    // counts how many players are in the standing request group
    private static int CountStandingPlayers(StandingTeeTimeRequest r)
    {
        int count = 1;
        if (!string.IsNullOrEmpty(r.Member2Name)) count++;
        if (!string.IsNullOrEmpty(r.Member3Name)) count++;
        if (!string.IsNullOrEmpty(r.Member4Name)) count++;
        return count;
    }
}
