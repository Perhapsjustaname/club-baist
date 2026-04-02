# Club BAIST

Golf course membership and tee time booking system built with Blazor Server and SQL Server.

---

## Tech Stack

| Layer          | Technology                                |
|----------------|-------------------------------------------|
| Frontend       | Blazor Server (.NET 9)                    |
| Backend        | ASP.NET Core                              |
| Database       | Microsoft SQL Server (local)              |
| ORM            | Entity Framework Core 9                   |
| Auth           | Custom session-based, BCrypt password hashing |

---

## Running Locally

### Prerequisites


- SQL Server (any edition — Express is fine) running on `localhost`
- SSMS or another SQL client (optional, for inspecting the DB)

### Steps

1. **Clone the repo**
   ```bash
   git clone <repo-url>
   cd club-baist
   ```

2. **Create your appsettings.json** 

   Create `ClubBaist/appsettings.json` with the following content:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ClubBaistDB;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```
  

3. **Run the app**
   ```bash
   cd ClubBaist
   dotnet run
   ```
   On first run, EF Core creates the `ClubBaistDB` database and seeds all sample data automatically.




---

## Sample Logins

### Admin & Staff

| Name                | Username    | Email                  | Password    | Role  | Member #     |
|---------------------|-------------|------------------------|-------------|-------|--------------|
| Club Administrator  | admin       | admin@clubbaist.ca     | Admin@1996  | Admin | CB-1970-0001 |
| Derek Thompson      | dthompson   | dthompson@clubbaist.ca | Member@123  | Staff | CB-2005-0002 |

### Gold Members

| Name          | Username | Email              | Password   | Type               | Member #     | Handicap |
|---------------|----------|--------------------|------------|--------------------|--------------|----------|
| James Smith   | jsmith   | jsmith@example.com | Member@123 | Shareholder        | CB-1997-0003 | 7.2      |
| Robert Jones  | rjones   | rjones@example.com | Member@123 | Associate          | CB-2008-0004 | 11.6     |

### Silver Members

| Name             | Username | Email               | Password   | Type               | Member #     | Handicap |
|------------------|----------|---------------------|------------|--------------------|--------------|----------|
| Margaret Wilson  | mwilson  | mwilson@example.com | Member@123 | Shareholder Spouse | CB-2001-0005 | 15.4     |
| Sarah Lee        | slee     | slee@example.com    | Member@123 | Associate Spouse   | CB-2010-0006 | 20.1     |

### Bronze Members

| Name          | Username  | Email                | Password   | Type         | Member #     | Handicap |
|---------------|-----------|----------------------|------------|--------------|--------------|----------|
| Karen Baker   | kbaker    | kbaker@example.com   | Member@123 | Intermediate | CB-2015-0007 | 16.2     |
| Tyler Johnson | tjohnson  | tjohnson@example.com | Member@123 | Junior       | CB-2018-0008 | 24.5     |

### Copper Members

| Name            | Username   | Email                 | Password   | Type   | Member #     |
|-----------------|------------|-----------------------|------------|--------|--------------|
| Pablo Martinez  | pmartinez  | pmartinez@example.com | Member@123 | Social | CB-2022-0009 |

> Copper (Social) members cannot book tee times.

---

## What the Seed Data Includes

### Player Rounds (2025 season)
| Member     | Rounds | Tees  | Notes                              |
|------------|--------|-------|------------------------------------|
| jsmith     | 10     | White | First 2 rounds include hole scores |
| dthompson  | 5      | White |                                    |
| rjones     | 8      | White |                                    |
| mwilson    | 6      | Red   |                                    |
| slee       | 4      | Red   |                                    |
| kbaker     | 4      | White |                                    |
| tjohnson   | 3      | White |                                    |

### Tee Time Bookings
- 4 past completed + checked-in bookings (March 1–8, 2026)
- 5 upcoming confirmed bookings (March 14–21, 2026)
- 4 upcoming standing bookings for rjones (Saturdays April 4–25, 2026, `IsStanding = true`)

### Standing Tee Time Requests
| Member  | Day       | Preferred Time | Status   | Notes                            |
|---------|-----------|----------------|----------|----------------------------------|
| jsmith  | Saturday  | 7:00 AM        | Pending  | Awaiting committee review        |
| rjones  | Saturday  | 9:00 AM        | Approved | Priority #1, bookings auto-created |

---

## Booking Time Restrictions

| Tier   | Weekdays                         | Weekends         |
|--------|----------------------------------|------------------|
| Gold   | Anytime                          | Anytime          |
| Silver | Before 3:00 PM or after 5:30 PM  | After 11:00 AM   |
| Bronze | Before 3:00 PM or after 6:00 PM  | After 1:00 PM    |
| Copper | No golf bookings                 | No golf bookings |

Golf season: **March 1 – September 30**. Tee times run 7:00 AM – 6:30 PM in 8-minute intervals.

> Season start was changed to March 1 (from April 1 per club rules) to allow earlier testing. Change `SeasonStart` in `TeeTimeService.cs` to revert.

---

## Annual Fees

| Tier   | Type                | Annual Fee |
|--------|---------------------|------------|
| Gold   | Shareholder         | $3,000     |
| Gold   | Associate           | $4,500     |
| Silver | Shareholder Spouse  | $2,000     |
| Silver | Associate Spouse    | $2,500     |
| Bronze | Intermediate        | $1,000     |
| Bronze | Junior              | $500       |
| Bronze | PeeWee              | $250       |
| Copper | Social              | $100       |

Entrance fee: $10,000. Share purchase: $1,000. Gold members have a $500 minimum food & beverage requirement. Fees do not include GST.
