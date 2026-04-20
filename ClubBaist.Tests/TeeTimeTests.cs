using ClubBaist.Models;
using ClubBaist.Services;
using Xunit;

namespace ClubBaist.Tests;

// Tests for TeeTimeService pure methods — no DB context needed
// Pass null! as the factory since these methods never touch it
public class TeeTimeTests
{
    private readonly TeeTimeService _svc = new TeeTimeService(null!);

    // ── IsInSeason ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(3, 1)]   // March 1 — season open
    [InlineData(6, 15)]  // mid-season
    [InlineData(9, 30)]  // September 30 — last day
    public void IsInSeason_DatesInsideSeason_ReturnsTrue(int month, int day)
    {
        var date = new DateOnly(DateTime.Now.Year, month, day);
        Assert.True(_svc.IsInSeason(date));
    }

    [Theory]
    [InlineData(1, 15)]  // January
    [InlineData(2, 28)]  // February — just before season
    [InlineData(10, 1)]  // October — day after season ends
    [InlineData(12, 25)] // Christmas, definitely closed
    public void IsInSeason_DatesOutsideSeason_ReturnsFalse(int month, int day)
    {
        var date = new DateOnly(DateTime.Now.Year, month, day);
        Assert.False(_svc.IsInSeason(date));
    }

    // ── GetAllSlots ───────────────────────────────────────────────────────────

    [Fact]
    public void GetAllSlots_FirstSlot_Is7AM()
    {
        var slots = _svc.GetAllSlots();
        Assert.Equal(new TimeOnly(7, 0), slots.First());
    }

    [Fact]
    public void GetAllSlots_AllSlotsEightMinutesApart()
    {
        var slots = _svc.GetAllSlots();
        for (int i = 1; i < slots.Count; i++)
        {
            int gap = (int)(slots[i] - slots[i - 1]).TotalMinutes;
            Assert.Equal(8, gap);
        }
    }

    [Fact]
    public void GetAllSlots_NoSlotAfter6_30PM()
    {
        var slots = _svc.GetAllSlots();
        Assert.All(slots, t => Assert.True(t <= new TimeOnly(18, 30)));
    }

    [Fact]
    public void GetAllSlots_CountIsCorrect()
    {
        // 7:00 to 18:28 in 8-min steps — 87 slots
        var slots = _svc.GetAllSlots();
        Assert.Equal(87, slots.Count);
    }

    // ── IsTimeAllowedForTier ──────────────────────────────────────────────────

    // Gold is always allowed
    [Theory]
    [InlineData(7, 0)]   // 7 AM weekday
    [InlineData(14, 0)]  // 2 PM (restricted for others)
    [InlineData(18, 30)] // 6:30 PM weekday
    public void IsTimeAllowed_Gold_AlwaysTrue(int hour, int minute)
    {
        // Tuesday = weekday
        var date = NextWeekday(DayOfWeek.Tuesday);
        var time = new TimeOnly(hour, minute);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Gold, date, time));
    }

    // Silver weekday: before 3 PM or at/after 5:30 PM → restricted window 3:00–5:29
    [Fact]
    public void IsTimeAllowed_Silver_Weekday_Before3PM_IsAllowed()
    {
        var date = NextWeekday(DayOfWeek.Wednesday);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Silver, date, new TimeOnly(14, 59)));
    }

    [Fact]
    public void IsTimeAllowed_Silver_Weekday_3PM_IsDenied()
    {
        var date = NextWeekday(DayOfWeek.Wednesday);
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Silver, date, new TimeOnly(15, 0)));
    }

    [Fact]
    public void IsTimeAllowed_Silver_Weekday_530PM_IsAllowed()
    {
        var date = NextWeekday(DayOfWeek.Wednesday);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Silver, date, new TimeOnly(17, 30)));
    }

    // Silver weekend: at/after 11 AM
    [Fact]
    public void IsTimeAllowed_Silver_Weekend_Before11AM_IsDenied()
    {
        var date = NextWeekday(DayOfWeek.Saturday);
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Silver, date, new TimeOnly(10, 59)));
    }

    [Fact]
    public void IsTimeAllowed_Silver_Weekend_11AM_IsAllowed()
    {
        var date = NextWeekday(DayOfWeek.Saturday);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Silver, date, new TimeOnly(11, 0)));
    }

    // Bronze weekday: before 3 PM or at/after 6 PM
    [Fact]
    public void IsTimeAllowed_Bronze_Weekday_Between3And6PM_IsDenied()
    {
        var date = NextWeekday(DayOfWeek.Thursday);
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Bronze, date, new TimeOnly(15, 30)));
    }

    [Fact]
    public void IsTimeAllowed_Bronze_Weekday_6PM_IsAllowed()
    {
        var date = NextWeekday(DayOfWeek.Thursday);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Bronze, date, new TimeOnly(18, 0)));
    }

    // Bronze weekend: at/after 1 PM
    [Fact]
    public void IsTimeAllowed_Bronze_Weekend_Before1PM_IsDenied()
    {
        var date = NextWeekday(DayOfWeek.Sunday);
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Bronze, date, new TimeOnly(12, 59)));
    }

    [Fact]
    public void IsTimeAllowed_Bronze_Weekend_1PM_IsAllowed()
    {
        var date = NextWeekday(DayOfWeek.Sunday);
        Assert.True(_svc.IsTimeAllowedForTier(MembershipTier.Bronze, date, new TimeOnly(13, 0)));
    }

    // Copper is never allowed
    [Fact]
    public void IsTimeAllowed_Copper_AlwaysDenied()
    {
        var weekday = NextWeekday(DayOfWeek.Monday);
        var weekend = NextWeekday(DayOfWeek.Saturday);
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Copper, weekday, new TimeOnly(10, 0)));
        Assert.False(_svc.IsTimeAllowedForTier(MembershipTier.Copper, weekend, new TimeOnly(14, 0)));
    }

    // ── helper ────────────────────────────────────────────────────────────────

    // returns the next occurrence of a given day of the week from today
    private static DateOnly NextWeekday(DayOfWeek target)
    {
        var d = DateOnly.FromDateTime(DateTime.Today);
        int daysUntil = ((int)target - (int)d.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7; // ensure it's in the future
        return d.AddDays(daysUntil);
    }
}
