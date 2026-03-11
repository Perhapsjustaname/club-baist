# CLUB-BAIST 

Welcome to the CLUB-BAIST web application. This project is a golf course booking system developed using Blazor and MS SQL Server.


## Tech Stack

* **Frontend**: Blazor 
* **Backend**: ASP.NET Core
* **Database**: Microsoft SQL Server
* **Authentication**: Custom Auth with BCrypt.Net-Next for password hashing

---

## Test Accounts

> **Note:** Sample data only runs on a fresh database. If you already have data, drop the `ClubBaistDB` database in SSMS and restart the app to reseed.

All member accounts use the password: `Member@123` for sample data

### Admin & Staff

| Name                | Username    | Email                    | Password    | Role  | Member #      |
|---------------------|-------------|--------------------------|-------------|-------|---------------|
| Club Administrator  | admin       | admin@clubbaist.ca       | Admin@1996  | Admin | CB-1970-0001  |
| Derek Thompson      | dthompson   | dthompson@clubbaist.ca   | Member@123  | Staff | CB-2005-0002  |

### Gold Members

| Name           | Username  | Email                  | Password   | Tier | Type               | Member #      | Handicap |
|----------------|-----------|------------------------|------------|------|--------------------|---------------|----------|
| James Smith    | jsmith    | jsmith@example.com     | Member@123 | Gold | Shareholder        | CB-1997-0003  | 7.2      |
| Robert Jones   | rjones    | rjones@example.com     | Member@123 | Gold | Associate          | CB-2008-0004  | 11.6     |

### Silver Members

| Name             | Username  | Email                  | Password   | Tier   | Type                 | Member #      | Handicap |
|------------------|-----------|------------------------|------------|--------|----------------------|---------------|----------|
| Margaret Wilson  | mwilson   | mwilson@example.com    | Member@123 | Silver | Shareholder Spouse   | CB-2001-0005  | 15.4     |
| Sarah Lee        | slee      | slee@example.com       | Member@123 | Silver | Associate Spouse     | CB-2010-0006  | 20.1     |

### Bronze Members

| Name          | Username   | Email                   | Password   | Tier   | Type          | Member #      | Handicap |
|---------------|------------|-------------------------|------------|--------|---------------|---------------|----------|
| Karen Baker   | kbaker     | kbaker@example.com      | Member@123 | Bronze | Intermediate  | CB-2015-0007  | 16.2     |
| Tyler Johnson | tjohnson   | tjohnson@example.com    | Member@123 | Bronze | Junior        | CB-2018-0008  | 24.5     |

### Copper Members

| Name            | Username    | Email                    | Password   | Tier   | Type    | Member #      | Handicap |
|-----------------|-------------|--------------------------|------------|--------|---------|---------------|----------|
| Pablo Martinez  | pmartinez   | pmartinez@example.com    | Member@123 | Copper | Social  | CB-2022-0009  | —        |

---

## Sample data inserted

- **Rounds:** 10 rounds for jsmith (2 with full hole-by-hole scorecards), 5 for dthompson, 8 for rjones, 6 for mwilson, 4 each for slee and kbaker, 3 for tjohnson in the 2025 season
- **Bookings:** 4 past completed bookings (March 1–8 2026) + 5 upcoming confirmed bookings (check smss for details)
- **Tee restrictions tested:** Silver weekend booking at 11am, Bronze weekend at 1pm 

---

## Booking Time Restrictions

| Tier   | Weekdays                        | Weekends        |
|--------|---------------------------------|-----------------|
| Gold   | Anytime                         | Anytime         |
| Silver | Before 3:00 PM or after 5:30 PM | After 11:00 AM  |
| Bronze | Before 3:00 PM or after 6:00 PM | After 1:00 PM   |
| Copper | No golf bookings                | No golf bookings|

Golf season runs **March 1 – September 30**. Tee times available 7:00 AM – 6:30 PM in 8-minute intervals. Chose March over April start due to testing needs for this web app. This date can be changed in the codebase. 

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

Entrance fee: $10,000. Share purchase: $1,000. Gold members have a $500 minimum food & beverage requirement.
