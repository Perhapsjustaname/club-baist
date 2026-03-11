namespace ClubBaist.Models;

public class PlayerRound
{
    public int RoundId { get; set; }
    public int MemberProfileId { get; set; }
    public MemberProfile MemberProfile { get; set; } = null!;

    public DateTime RoundDate { get; set; }
    public string CourseName { get; set; } = "Club BAIST";
    public string TeeColor { get; set; } = "White"; // Red, White, Blue

    // Course rating info (from scorecard or Golf Canada approved course)
    public decimal CourseRating { get; set; }
    public int SlopeRating { get; set; }
    public int Par { get; set; } = 72;

    public int TotalScore { get; set; }

    // WHS Score Differential = (Score - Course Rating) x 113 / Slope Rating
    public decimal ScoreDifferential { get; set; }

    public bool IsApprovedCourse { get; set; } = true;
    public DateTime EnteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<HoleScore> HoleScores { get; set; } = new List<HoleScore>();
}
