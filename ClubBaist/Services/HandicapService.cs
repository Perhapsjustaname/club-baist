using ClubBaist.Models;

namespace ClubBaist.Services;

// static bc theres no state here
public static class HandicapService
{
    // Club BAIST course data (from scorecard)
    public static readonly Dictionary<string, (decimal MenRating, int MenSlope, decimal WomenRating, int WomenSlope, int Par)> ClubBaistTees = new()
    {
        ["Red"]   = (66.2m, 116, 71.0m, 125, 71),
        ["White"] = (68.8m, 123, 75.0m, 133, 72),
        ["Blue"]  = (70.9m, 127, 76.6m, 138, 72)
    };

    // Club BAIST par per hole (Men's standard)
    public static readonly int[] HolePars = { 4, 5, 3, 4, 4, 4, 3, 5, 4, 4, 4, 3, 5, 4, 4, 3, 4, 4 };

    // Club BAIST stroke index (Men)
    public static readonly int[] MenStrokeIndex = { 1, 5, 17, 11, 9, 7, 15, 3, 13, 12, 6, 16, 2, 10, 14, 18, 4, 8 };

    // WHS formula: (score - courseRating) x 113 / slope, rounded to 1 decimal
    // 113 is the "standard" slope used as the reference point by Golf Canada
    public static decimal CalculateScoreDifferential(int score, decimal courseRating, int slopeRating)
    {
        decimal result = Math.Round((score - courseRating) * 113m / slopeRating, 1);
        return result;
    }

    // best 8 of last 20 x 0.96 = your handicap index (WHS rule)
    // returns 0 if less than 3 rounds - need at least 3 before we calc anything
    public static decimal CalculateHandicapIndex(IEnumerable<PlayerRound> allRounds)
    {
        var last20 = allRounds
            .OrderByDescending(r => r.RoundDate)
            .Take(20)
            .ToList();

        if (last20.Count < 3) return 0m;

        var differentials = last20
            .Select(r => r.ScoreDifferential)
            .OrderBy(d => d)
            .ToList();

        int take = GetTakeCount(last20.Count);
        if (take == 0) return 0m;

        var avg = differentials.Take(take).Average();
        return Math.Round((decimal)avg * 0.96m, 1);
    }

    // breakdown version used for display, returns last20, best8, and the index all at once
    public static (List<decimal> Last20, List<decimal> Best8, decimal HandicapIndex) GetHandicapBreakdown(IEnumerable<PlayerRound> allRounds)
    {
        var last20 = allRounds
            .OrderByDescending(r => r.RoundDate)
            .Take(20)
            .ToList();

        var differentials = last20.Select(r => r.ScoreDifferential).ToList();

        var sorted = differentials.OrderBy(d => d).ToList();

        int take = last20.Count < 3 ? 0 : GetTakeCount(last20.Count);

        List<decimal> best8 = sorted.Take(take).ToList();
        var index = take > 0
            ? Math.Round((decimal)best8.Average() * 0.96m, 1)
            : 0m;

        return (differentials, best8, index);
    }

    // lookup table from the WHS spec - how many differentials to use based on rounds played
    // once you hit 20 rounds it maxes out at 8, below 3 we dont calculate at all 
    //TODO needs testing
    private static int GetTakeCount(int roundCount) => roundCount switch
    {
        <= 2       => 0,
        3 or 4     => 1,
        5 or 6     => 2,
        7 or 8 or 9 => 3,
        10 or 11   => 3,
        12 or 13 or 14 => 4,
        15         => 5,
        16 or 17   => 6,
        18 or 19   => 7,
        _          => 8  // 20+
    };
}
