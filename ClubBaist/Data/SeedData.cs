using ClubBaist.Models;

namespace ClubBaist.Data;

// all dev/demo seed data lives here, not in Program.cs
// called once on startup when the db is brand new (no users)
public static class SeedData
{
    public static void Initialize(ClubBaistDbContext db)
    {
        // guard: dont run if db already has data
        if (db.Users.Any()) return;

        // ============================================================
        // USERS + MEMBER PROFILES
        // ============================================================

        // ---- ADMIN ----
        var adminUser = new User
        {
            Username     = "admin",
            Email        = "admin@clubbaist.ca",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1996"),
            Role         = "Admin"
        };
        db.Users.Add(adminUser);
        db.SaveChanges();

        // admin profile so they show up in member lists and have a member number
        var adminProfile = new MemberProfile
        {
            UserId       = adminUser.UserId,
            FirstName    = "Club",
            LastName     = "Administrator",
            MemberNumber = "CB-1970-0001",
            Tier         = MembershipTier.Gold,
            MemberType   = MembershipType.Shareholder,
            Status       = MemberStatus.Active,
            Phone        = "(403) 555-0100",
            Address      = "1 Golf Course Drive",
            PostalCode   = "T2P 1A1",
            MemberSince  = new DateTime(1970, 6, 1),
            HandicapIndex = 4.2m
        };
        db.MemberProfiles.Add(adminProfile);
        db.SaveChanges();

        // ---- STAFF: Derek Thompson ----
        var staffUser = new User
        {
            Username     = "dthompson",
            Email        = "dthompson@clubbaist.ca",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Staff"
        };
        db.Users.Add(staffUser);
        db.SaveChanges();

        var staffProfile = new MemberProfile
        {
            UserId        = staffUser.UserId,
            FirstName     = "Derek",
            LastName      = "Thompson",
            MemberNumber  = "CB-2005-0002",
            Tier          = MembershipTier.Gold,
            MemberType    = MembershipType.Shareholder,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0102",
            Address       = "42 Fairway Crescent",
            PostalCode    = "T2P 2B2",
            DateOfBirth   = new DateOnly(1979, 11, 3),
            Occupation    = "Club Pro",
            MemberSince   = new DateTime(2005, 4, 1),
            HandicapIndex = 5.4m
        };
        db.MemberProfiles.Add(staffProfile);
        db.SaveChanges();

        // ---- GOLD SHAREHOLDER: James Smith ----
        var goldUser = new User
        {
            Username     = "jsmith",
            Email        = "jsmith@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Gold"
        };
        db.Users.Add(goldUser);
        db.SaveChanges();

        var goldProfile = new MemberProfile
        {
            UserId        = goldUser.UserId,
            FirstName     = "James",
            LastName      = "Smith",
            MemberNumber  = "CB-1997-0003",
            Tier          = MembershipTier.Gold,
            MemberType    = MembershipType.Shareholder,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0201",
            Address       = "114 Eagle Ridge Way",
            PostalCode    = "T3H 1C4",
            DateOfBirth   = new DateOnly(1968, 3, 15),
            Occupation    = "Engineer",
            CompanyName   = "Foothills Engineering Ltd.",
            MemberSince   = new DateTime(1997, 4, 1),
            HandicapIndex = 7.2m
        };
        db.MemberProfiles.Add(goldProfile);
        db.SaveChanges();

        // ---- GOLD ASSOCIATE: Robert Jones ----
        var rjonesUser = new User
        {
            Username     = "rjones",
            Email        = "rjones@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Gold"
        };
        db.Users.Add(rjonesUser);
        db.SaveChanges();

        var rjonesProfile = new MemberProfile
        {
            UserId        = rjonesUser.UserId,
            FirstName     = "Robert",
            LastName      = "Jones",
            MemberNumber  = "CB-2008-0004",
            Tier          = MembershipTier.Gold,
            MemberType    = MembershipType.Associate,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0203",
            Address       = "27 Birchwood Lane",
            PostalCode    = "T2S 3E7",
            DateOfBirth   = new DateOnly(1975, 8, 22),
            Occupation    = "Lawyer",
            CompanyName   = "Jones & Partners LLP",
            MemberSince   = new DateTime(2008, 4, 1),
            HandicapIndex = 11.6m
        };
        db.MemberProfiles.Add(rjonesProfile);
        db.SaveChanges();

        // ---- SILVER SHAREHOLDER SPOUSE: Margaret Wilson ----
        var silverUser = new User
        {
            Username     = "mwilson",
            Email        = "mwilson@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Silver"
        };
        db.Users.Add(silverUser);
        db.SaveChanges();

        var silverProfile = new MemberProfile
        {
            UserId        = silverUser.UserId,
            FirstName     = "Margaret",
            LastName      = "Wilson",
            MemberNumber  = "CB-2001-0005",
            Tier          = MembershipTier.Silver,
            MemberType    = MembershipType.ShareholderSpouse,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0202",
            Address       = "88 Willow Park Drive",
            PostalCode    = "T2J 5K3",
            DateOfBirth   = new DateOnly(1972, 7, 22),
            MemberSince   = new DateTime(2001, 4, 1),
            HandicapIndex = 15.4m
        };
        db.MemberProfiles.Add(silverProfile);
        db.SaveChanges();

        // ---- SILVER ASSOCIATE SPOUSE: Sarah Lee ----
        var sleeUser = new User
        {
            Username     = "slee",
            Email        = "slee@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Silver"
        };
        db.Users.Add(sleeUser);
        db.SaveChanges();

        var sleeProfile = new MemberProfile
        {
            UserId        = sleeUser.UserId,
            FirstName     = "Sarah",
            LastName      = "Lee",
            MemberNumber  = "CB-2010-0006",
            Tier          = MembershipTier.Silver,
            MemberType    = MembershipType.AssociateSpouse,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0310",
            Address       = "55 Harvest Hills Blvd",
            PostalCode    = "T3K 4L2",
            DateOfBirth   = new DateOnly(1985, 2, 14),
            MemberSince   = new DateTime(2010, 5, 1),
            HandicapIndex = 20.1m
        };
        db.MemberProfiles.Add(sleeProfile);
        db.SaveChanges();

        // ---- BRONZE INTERMEDIATE: Karen Baker ----
        var kbakerUser = new User
        {
            Username     = "kbaker",
            Email        = "kbaker@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Bronze"
        };
        db.Users.Add(kbakerUser);
        db.SaveChanges();

        var kbakerProfile = new MemberProfile
        {
            UserId        = kbakerUser.UserId,
            FirstName     = "Karen",
            LastName      = "Baker",
            MemberNumber  = "CB-2015-0007",
            Tier          = MembershipTier.Bronze,
            MemberType    = MembershipType.Intermediate,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0412",
            Address       = "301 Copperfield Blvd",
            PostalCode    = "T2Z 4R9",
            DateOfBirth   = new DateOnly(1998, 9, 5),
            MemberSince   = new DateTime(2015, 6, 1),
            HandicapIndex = 16.2m
        };
        db.MemberProfiles.Add(kbakerProfile);
        db.SaveChanges();

        // ---- BRONZE JUNIOR: Tyler Johnson ----
        var tjohnsonUser = new User
        {
            Username     = "tjohnson",
            Email        = "tjohnson@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Bronze"
        };
        db.Users.Add(tjohnsonUser);
        db.SaveChanges();

        var tjohnsonProfile = new MemberProfile
        {
            UserId        = tjohnsonUser.UserId,
            FirstName     = "Tyler",
            LastName      = "Johnson",
            MemberNumber  = "CB-2018-0008",
            Tier          = MembershipTier.Bronze,
            MemberType    = MembershipType.Junior,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0501",
            Address       = "12 Cranston Circle",
            PostalCode    = "T3M 1S8",
            DateOfBirth   = new DateOnly(2006, 4, 18),
            MemberSince   = new DateTime(2018, 7, 1),
            HandicapIndex = 24.5m
        };
        db.MemberProfiles.Add(tjohnsonProfile);
        db.SaveChanges();

        // ---- COPPER SOCIAL: Pablo Martinez ----
        // social membership, cant book tee times, just club access
        var pmartinezUser = new User
        {
            Username     = "pmartinez",
            Email        = "pmartinez@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
            Role         = "Copper"
        };
        db.Users.Add(pmartinezUser);
        db.SaveChanges();

        var pmartinezProfile = new MemberProfile
        {
            UserId        = pmartinezUser.UserId,
            FirstName     = "Pablo",
            LastName      = "Martinez",
            MemberNumber  = "CB-2022-0009",
            Tier          = MembershipTier.Copper,
            MemberType    = MembershipType.Social,
            Status        = MemberStatus.Active,
            Phone         = "(403) 555-0602",
            Address       = "77 Auburn Bay Drive",
            PostalCode    = "T3M 2G4",
            DateOfBirth   = new DateOnly(1990, 12, 30),
            MemberSince   = new DateTime(2022, 3, 1),
            HandicapIndex = null // social members dont golf
        };
        db.MemberProfiles.Add(pmartinezProfile);
        db.SaveChanges();

        // ============================================================
        // PLAYER ROUNDS + HOLE SCORES
        // White tees: CourseRating=71.5, Slope=128
        // Red tees:   CourseRating=68.5, Slope=115
        // ScoreDifferential = (Score - CR) x 113 / Slope
        // ============================================================

        // jsmith — 10 rounds (2025 season, white tees)
        // first two rounds have full hole-by-hole scorecards
        var jsmithRound1 = new PlayerRound
        {
            MemberProfileId   = goldProfile.MemberProfileId,
            RoundDate         = new DateTime(2025, 5, 3),
            CourseName        = "Club BAIST",
            TeeColor          = "White",
            CourseRating      = 71.5m,
            SlopeRating       = 128,
            Par               = 72,
            TotalScore        = 82,
            ScoreDifferential = 9.27m,
            IsApprovedCourse  = true,
            HoleScores = new List<HoleScore>
            {
                new HoleScore { HoleNumber=1,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=2,  Par=3, Score=3, Putts=2 },
                new HoleScore { HoleNumber=3,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=4,  Par=5, Score=6, Putts=2 },
                new HoleScore { HoleNumber=5,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=6,  Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=7,  Par=3, Score=4, Putts=2 },
                new HoleScore { HoleNumber=8,  Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=9,  Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=10, Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=11, Par=3, Score=3, Putts=1 },
                new HoleScore { HoleNumber=12, Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=13, Par=5, Score=6, Putts=2 },
                new HoleScore { HoleNumber=14, Par=3, Score=3, Putts=1 },
                new HoleScore { HoleNumber=15, Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=16, Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=17, Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=18, Par=4, Score=5, Putts=2 },
            }
        };

        var jsmithRound2 = new PlayerRound
        {
            MemberProfileId   = goldProfile.MemberProfileId,
            RoundDate         = new DateTime(2025, 5, 17),
            CourseName        = "Club BAIST",
            TeeColor          = "White",
            CourseRating      = 71.5m,
            SlopeRating       = 128,
            Par               = 72,
            TotalScore        = 79,
            ScoreDifferential = 6.62m,
            IsApprovedCourse  = true,
            HoleScores = new List<HoleScore>
            {
                new HoleScore { HoleNumber=1,  Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=2,  Par=3, Score=3, Putts=2 },
                new HoleScore { HoleNumber=3,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=4,  Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=5,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=6,  Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=7,  Par=3, Score=3, Putts=1 },
                new HoleScore { HoleNumber=8,  Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=9,  Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=10, Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=11, Par=3, Score=3, Putts=1 },
                new HoleScore { HoleNumber=12, Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=13, Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=14, Par=3, Score=4, Putts=2 },
                new HoleScore { HoleNumber=15, Par=4, Score=5, Putts=2 },
                new HoleScore { HoleNumber=16, Par=4, Score=4, Putts=1 },
                new HoleScore { HoleNumber=17, Par=5, Score=5, Putts=2 },
                new HoleScore { HoleNumber=18, Par=4, Score=5, Putts=2 },
            }
        };

        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            jsmithRound1,
            jsmithRound2,
            // remaining jsmith rounds (totals only, no hole detail)
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,6,7),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=84, ScoreDifferential=11.04m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,6,21), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=80, ScoreDifferential=7.50m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,7,5),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=77, ScoreDifferential=4.86m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,7,19), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=85, ScoreDifferential=11.92m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,8,2),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=81, ScoreDifferential=8.39m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,8,16), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=83, ScoreDifferential=10.15m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,8,30), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=78, ScoreDifferential=5.74m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=goldProfile.MemberProfileId, RoundDate=new DateTime(2025,9,13), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=80, ScoreDifferential=7.50m,  IsApprovedCourse=true },
        });

        // dthompson — 5 rounds (staff, white tees)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=staffProfile.MemberProfileId, RoundDate=new DateTime(2025,5,10), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=77, ScoreDifferential=4.86m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=staffProfile.MemberProfileId, RoundDate=new DateTime(2025,6,14), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=79, ScoreDifferential=6.62m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=staffProfile.MemberProfileId, RoundDate=new DateTime(2025,7,12), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=78, ScoreDifferential=5.74m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=staffProfile.MemberProfileId, RoundDate=new DateTime(2025,8,9),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=76, ScoreDifferential=3.97m,  IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=staffProfile.MemberProfileId, RoundDate=new DateTime(2025,9,6),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=80, ScoreDifferential=7.50m,  IsApprovedCourse=true },
        });

        // rjones — 8 rounds (white tees)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,4,19), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=84, ScoreDifferential=11.04m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,5,24), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=87, ScoreDifferential=13.68m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,6,7),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=83, ScoreDifferential=10.15m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,6,28), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=85, ScoreDifferential=11.92m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,7,19), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=86, ScoreDifferential=12.80m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,8,2),  CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=88, ScoreDifferential=14.57m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,8,23), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=84, ScoreDifferential=11.04m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=rjonesProfile.MemberProfileId, RoundDate=new DateTime(2025,9,13), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=82, ScoreDifferential=9.27m,  IsApprovedCourse=true },
        });

        // mwilson — 6 rounds (red tees, CR=68.5 Slope=115)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,6,7),  CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=87, ScoreDifferential=18.18m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,6,28), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=90, ScoreDifferential=21.13m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,7,12), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=85, ScoreDifferential=16.21m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,7,26), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=88, ScoreDifferential=19.16m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,8,9),  CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=86, ScoreDifferential=17.19m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=silverProfile.MemberProfileId, RoundDate=new DateTime(2025,8,23), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=92, ScoreDifferential=23.09m, IsApprovedCourse=true },
        });

        // slee — 4 rounds (red tees)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=sleeProfile.MemberProfileId, RoundDate=new DateTime(2025,7,5),  CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=92, ScoreDifferential=23.09m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=sleeProfile.MemberProfileId, RoundDate=new DateTime(2025,7,26), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=95, ScoreDifferential=26.04m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=sleeProfile.MemberProfileId, RoundDate=new DateTime(2025,8,16), CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=89, ScoreDifferential=20.14m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=sleeProfile.MemberProfileId, RoundDate=new DateTime(2025,9,6),  CourseName="Club BAIST", TeeColor="Red", CourseRating=68.5m, SlopeRating=115, Par=72, TotalScore=94, ScoreDifferential=25.06m, IsApprovedCourse=true },
        });

        // kbaker — 4 rounds (white tees)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=kbakerProfile.MemberProfileId, RoundDate=new DateTime(2025,5,17), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=88, ScoreDifferential=14.57m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=kbakerProfile.MemberProfileId, RoundDate=new DateTime(2025,6,21), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=91, ScoreDifferential=17.21m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=kbakerProfile.MemberProfileId, RoundDate=new DateTime(2025,7,26), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=87, ScoreDifferential=13.68m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=kbakerProfile.MemberProfileId, RoundDate=new DateTime(2025,8,30), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=90, ScoreDifferential=16.33m, IsApprovedCourse=true },
        });

        // tjohnson — 3 rounds (junior, still learning)
        db.PlayerRounds.AddRange(new List<PlayerRound>
        {
            new PlayerRound { MemberProfileId=tjohnsonProfile.MemberProfileId, RoundDate=new DateTime(2025,6,14), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=96, ScoreDifferential=21.63m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=tjohnsonProfile.MemberProfileId, RoundDate=new DateTime(2025,7,19), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=98, ScoreDifferential=23.39m, IsApprovedCourse=true },
            new PlayerRound { MemberProfileId=tjohnsonProfile.MemberProfileId, RoundDate=new DateTime(2025,8,23), CourseName="Club BAIST", TeeColor="White", CourseRating=71.5m, SlopeRating=128, Par=72, TotalScore=94, ScoreDifferential=19.86m, IsApprovedCourse=true },
        });

        db.SaveChanges();

        // ============================================================
        // TEE TIME BOOKINGS
        // past = Completed (checked in), upcoming April 2026 = Confirmed
        // ============================================================
        db.TeeTimeBookings.AddRange(new List<TeeTimeBooking>
        {
            // past bookings (completed + checked in)
            new TeeTimeBooking
            {
                MemberProfileId = goldProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 1),
                TeeTime         = new TimeOnly(7, 0),
                NumberOfPlayers = 4,
                NumberOfCarts   = 2,
                Player2Name     = "Robert Jones",
                Player3Name     = "Derek Thompson",
                Player4Name     = "Guest - B. Fraser",
                Status          = BookingStatus.Completed,
                CheckedIn       = true,
                CheckedInAt     = new DateTime(2026, 3, 1, 6, 52, 0, DateTimeKind.Utc),
                BookedAt        = new DateTime(2026, 2, 26, 14, 0, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 3),
                TeeTime         = new TimeOnly(8, 0),
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Margaret Wilson",
                Status          = BookingStatus.Completed,
                CheckedIn       = true,
                CheckedInAt     = new DateTime(2026, 3, 3, 7, 55, 0, DateTimeKind.Utc),
                BookedAt        = new DateTime(2026, 2, 27, 10, 30, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = staffProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 5),
                TeeTime         = new TimeOnly(7, 0),
                NumberOfPlayers = 1,
                NumberOfCarts   = 0,
                Notes           = "walking - early morning round before shift",
                Status          = BookingStatus.Completed,
                CheckedIn       = true,
                CheckedInAt     = new DateTime(2026, 3, 5, 6, 58, 0, DateTimeKind.Utc),
                BookedAt        = new DateTime(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = silverProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 8),  // saturday, after 11am = ok for silver
                TeeTime         = new TimeOnly(11, 0),
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Sarah Lee",
                Status          = BookingStatus.Completed,
                CheckedIn       = true,
                CheckedInAt     = new DateTime(2026, 3, 8, 10, 57, 0, DateTimeKind.Utc),
                BookedAt        = new DateTime(2026, 3, 4, 16, 0, 0, DateTimeKind.Utc),
            },

            // upcoming confirmed bookings
            new TeeTimeBooking
            {
                MemberProfileId = goldProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 14),
                TeeTime         = new TimeOnly(7, 0),
                NumberOfPlayers = 3,
                NumberOfCarts   = 1,
                Player2Name     = "Robert Jones",
                Player3Name     = "Guest - T. Nguyen",
                Status          = BookingStatus.Confirmed,
                BookedAt        = new DateTime(2026, 3, 8, 11, 0, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = staffProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 14),
                TeeTime         = new TimeOnly(7, 8),  // 8 min after jsmith on same tee sheet
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Karen Baker",
                Status          = BookingStatus.Confirmed,
                BookedAt        = new DateTime(2026, 3, 7, 9, 15, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 15),
                TeeTime         = new TimeOnly(8, 0),
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Guest - P. Okafor",
                Notes           = "guest visiting from Toronto, treat well",
                Status          = BookingStatus.Confirmed,
                BookedAt        = new DateTime(2026, 3, 9, 14, 30, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = silverProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 16),  // monday, before 3pm normally restricted for silver but booked by staff
                TeeTime         = new TimeOnly(10, 0),
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Sarah Lee",
                Notes           = "ladies day - booked by staff",
                Status          = BookingStatus.Confirmed,
                BookedByStaff   = "dthompson",
                BookedAt        = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
            },
            new TeeTimeBooking
            {
                MemberProfileId = kbakerProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 3, 21),  // saturday, after 1pm = ok for bronze
                TeeTime         = new TimeOnly(13, 0),
                NumberOfPlayers = 2,
                NumberOfCarts   = 1,
                Player2Name     = "Tyler Johnson",
                Status          = BookingStatus.Confirmed,
                BookedAt        = new DateTime(2026, 3, 14, 17, 45, 0, DateTimeKind.Utc),
            },
        });

        db.SaveChanges();

        // ============================================================
        // STANDING TEE TIME REQUESTS
        // jsmith: pending Saturday request, awaiting committee review
        // rjones: approved Saturday 9:00 AM slot, priority #1
        // ============================================================

        // jsmith submitted a request for Saturdays - hasn't been reviewed yet
        db.StandingTeeTimeRequests.Add(new StandingTeeTimeRequest
        {
            PrimaryMemberProfileId = goldProfile.MemberProfileId,
            RequestedDayOfWeek     = DayOfWeek.Saturday,
            RequestedTime          = new TimeOnly(7, 0),
            StartDate              = new DateOnly(2026, 4, 1),
            EndDate                = new DateOnly(2026, 9, 30),
            Member2Name            = "Robert Jones",
            Member2Number          = "CB-2008-0004",
            Member3Name            = "Derek Thompson",
            Member3Number          = "CB-2005-0002",
            Status                 = RequestStatus.Pending,
            IsActive               = false
        });

        // rjones approved Saturday slot at 9:00 AM, priority #1 for the 2026 season
        db.StandingTeeTimeRequests.Add(new StandingTeeTimeRequest
        {
            PrimaryMemberProfileId = rjonesProfile.MemberProfileId,
            RequestedDayOfWeek     = DayOfWeek.Saturday,
            RequestedTime          = new TimeOnly(9, 0),
            StartDate              = new DateOnly(2026, 4, 1),
            EndDate                = new DateOnly(2026, 9, 30),
            Member2Name            = "Margaret Wilson",
            Member2Number          = "CB-2001-0005",
            Status                 = RequestStatus.Approved,
            PriorityNumber         = 1,
            ApprovedTime           = new TimeOnly(9, 0),
            ApprovedBy             = "admin",
            ApprovedDate           = new DateTime(2026, 3, 25, 14, 0, 0, DateTimeKind.Utc),
            IsActive               = true
        });

        db.SaveChanges();

        // standing bookings for rjones - first handful of upcoming Saturdays
        // (full season would be 26 Saturdays, just seeding a few to show the feature)
        db.TeeTimeBookings.AddRange(new List<TeeTimeBooking>
        {
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 4, 4),
                TeeTime         = new TimeOnly(9, 0),
                NumberOfPlayers = 2,
                Player2Name     = "Margaret Wilson",
                Status          = BookingStatus.Confirmed,
                IsStanding      = true,
                BookedAt        = new DateTime(2026, 3, 25, 14, 0, 0, DateTimeKind.Utc),
                BookedByStaff   = "admin"
            },
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 4, 11),
                TeeTime         = new TimeOnly(9, 0),
                NumberOfPlayers = 2,
                Player2Name     = "Margaret Wilson",
                Status          = BookingStatus.Confirmed,
                IsStanding      = true,
                BookedAt        = new DateTime(2026, 3, 25, 14, 0, 0, DateTimeKind.Utc),
                BookedByStaff   = "admin"
            },
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 4, 18),
                TeeTime         = new TimeOnly(9, 0),
                NumberOfPlayers = 2,
                Player2Name     = "Margaret Wilson",
                Status          = BookingStatus.Confirmed,
                IsStanding      = true,
                BookedAt        = new DateTime(2026, 3, 25, 14, 0, 0, DateTimeKind.Utc),
                BookedByStaff   = "admin"
            },
            new TeeTimeBooking
            {
                MemberProfileId = rjonesProfile.MemberProfileId,
                TeeDate         = new DateOnly(2026, 4, 25),
                TeeTime         = new TimeOnly(9, 0),
                NumberOfPlayers = 2,
                Player2Name     = "Margaret Wilson",
                Status          = BookingStatus.Confirmed,
                IsStanding      = true,
                BookedAt        = new DateTime(2026, 3, 25, 14, 0, 0, DateTimeKind.Utc),
                BookedByStaff   = "admin"
            },
        });

        db.SaveChanges();
    }
}
