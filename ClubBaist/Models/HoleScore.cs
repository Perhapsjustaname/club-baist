namespace ClubBaist.Models;

public class HoleScore
{
    public int HoleScoreId { get; set; }
    public int RoundId { get; set; }
    public PlayerRound Round { get; set; } = null!;

    public int HoleNumber { get; set; }
    public int Par { get; set; }
    public int Score { get; set; }
    public int? Putts { get; set; }

    public int RelativeToPar => Score - Par;
}
