using ClubBaist.Models;
using ClubBaist.Services;
using Xunit;

namespace ClubBaist.Tests;

// Tests for HandicapService — fully static, no DB needed
public class HandicapTests
{
    // ── CalculateScoreDifferential ────────────────────────────────────────────

    [Fact]
    public void ScoreDifferential_TypicalRound_CorrectValue()
    {
        // score 82, White tees (rating 68.8, slope 123) → (82-68.8)*113/123 = 12.1
        decimal result = HandicapService.CalculateScoreDifferential(82, 68.8m, 123);
        Assert.Equal(12.1m, result);
    }

    [Fact]
    public void ScoreDifferential_ScoreBelowRating_NegativeDifferential()
    {
        // a scratch golfer shooting under course rating gets a negative differential
        decimal result = HandicapService.CalculateScoreDifferential(67, 68.8m, 123);
        Assert.True(result < 0m);
    }

    [Fact]
    public void ScoreDifferential_ScoreEqualsRating_ZeroDifferential()
    {
        // score exactly at course rating → 0.0 regardless of slope
        decimal result = HandicapService.CalculateScoreDifferential(69, 69.0m, 130);
        Assert.Equal(0.0m, result);
    }

    // ── CalculateHandicapIndex ────────────────────────────────────────────────

    [Fact]
    public void HandicapIndex_FewerThanThreeRounds_ReturnsZero()
    {
        var rounds = MakeRounds(2, 10.0m);
        Assert.Equal(0m, HandicapService.CalculateHandicapIndex(rounds));
    }

    [Fact]
    public void HandicapIndex_ExactlyThreeRounds_UsesBestOne()
    {
        // 3 rounds → take best 1, multiply by 0.96
        var rounds = MakeRounds(differentials: new[] { 8.0m, 12.0m, 15.0m });
        decimal expected = Math.Round(8.0m * 0.96m, 1); // 7.7
        Assert.Equal(expected, HandicapService.CalculateHandicapIndex(rounds));
    }

    [Fact]
    public void HandicapIndex_TwentyRounds_UsesBestEight()
    {
        // 20 rounds, all same differential → avg of best 8 = same value × 0.96
        var rounds = MakeRounds(20, 10.0m);
        decimal expected = Math.Round(10.0m * 0.96m, 1); // 9.6
        Assert.Equal(expected, HandicapService.CalculateHandicapIndex(rounds));
    }

    [Fact]
    public void HandicapIndex_OnlyBestRoundsUsed_HighScoresIgnored()
    {
        // 20 rounds: 8 good (5.0) + 12 bad (30.0) — index should only reflect the 8 good ones
        var good  = MakeRounds(8,  5.0m);
        var bad   = MakeRounds(12, 30.0m);
        var all   = good.Concat(bad).ToList();
        decimal expected = Math.Round(5.0m * 0.96m, 1); // 4.8
        Assert.Equal(expected, HandicapService.CalculateHandicapIndex(all));
    }

    // ── GetTakeCount (tested indirectly through CalculateHandicapIndex) ───────

    [Theory]
    [InlineData(3,  1)]
    [InlineData(4,  1)]
    [InlineData(5,  2)]
    [InlineData(6,  2)]
    [InlineData(7,  3)]
    [InlineData(11, 3)]
    [InlineData(12, 4)]
    [InlineData(14, 4)]
    [InlineData(15, 5)]
    [InlineData(16, 6)]
    [InlineData(17, 6)]
    [InlineData(18, 7)]
    [InlineData(19, 7)]
    [InlineData(20, 8)]
    public void GetTakeCount_VariousRoundCounts_CorrectBestNUsed(int roundCount, int expectedBest)
    {
        // build rounds where the best N have differential 1.0 and the rest 99.0
        // if the correct take count is used, the average of best-N differentials is 1.0
        // and the handicap = round(1.0 × 0.96, 1) = 1.0
        var rounds = new List<PlayerRound>();
        for (int i = 0; i < expectedBest; i++)
            rounds.Add(MakeRound(1.0m, daysAgo: i));
        for (int i = expectedBest; i < roundCount; i++)
            rounds.Add(MakeRound(99.0m, daysAgo: i + roundCount)); // older, won't be best

        decimal index = HandicapService.CalculateHandicapIndex(rounds);
        decimal expected = Math.Round(1.0m * 0.96m, 1);
        Assert.Equal(expected, index);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    // build N rounds with the same differential, dates descending from today
    private static List<PlayerRound> MakeRounds(int count, decimal differential)
    {
        var rounds = new List<PlayerRound>();
        for (int i = 0; i < count; i++)
            rounds.Add(MakeRound(differential, daysAgo: i));
        return rounds;
    }

    // build rounds from an explicit array of differentials
    private static List<PlayerRound> MakeRounds(decimal[] differentials)
    {
        var rounds = new List<PlayerRound>();
        for (int i = 0; i < differentials.Length; i++)
            rounds.Add(MakeRound(differentials[i], daysAgo: i));
        return rounds;
    }

    private static PlayerRound MakeRound(decimal differential, int daysAgo = 0) =>
        new PlayerRound
        {
            MemberProfileId   = 1,
            MemberProfile     = null!,
            RoundDate         = DateTime.Today.AddDays(-daysAgo),
            TotalScore        = 80,
            CourseRating      = 68.8m,
            SlopeRating       = 123,
            ScoreDifferential = differential
        };
}
