# Project Notes

## 1. Scope

This document records implementation details that can be verified from the current repository.

The project is a Windows Forms cinema booking system created in 2020. The repository contains the UI source project and several binary dependencies, but does not contain the original SQL Server database definition or the source projects for all referenced libraries.

## 2. Functional map

### Customer side

| Component | Responsibility |
| --- | --- |
| `Form1` | Customer login and application entry |
| `Form2` | Customer booking menu |
| `Form3` | Customer registration |
| `Form4` | Movie information and showtime selection |
| `BuyTickets` | Seat rendering, selection, availability checking, and order insertion |
| `Form5` | Order history and order operations |
| `Form6` | Membership / VIP operations |

### Administrator side

| Component | Responsibility |
| --- | --- |
| `Form7` | Administrator login |
| `Form8` | Administrator navigation |
| `Form9` | Hall management |
| `Form10` | Movie management |
| `Form11` | Showtime management |
| `Form12` | Order management |
| `Form13` | Customer management |
| `Form14` | Statistics navigation |
| `Form15` | Statistics-related view |
| `Form17` | Statistics-related view |

`Form16` and `Form18` are retained in the source tree. Their exact runtime role has not been documented here because the current repository does not provide enough context to identify it with confidence.

## 3. Project dependencies

The checked-in project is `hh.csproj`, targeting:

- .NET Framework 4.0 Client Profile
- x86
- Windows Forms

The project depends on:

- `Model.dll`
- `PublicLib.dll`
- `IrisSkin4.dll`
- `MP10.ssk`
- SQL Server through `System.Data.SqlClient`

The original `hh.csproj` referenced `Model` and `PublicLib` as sibling source projects outside this repository. Their source projects are not present. The DLLs that were available in the original build output are now stored under `lib/`.

## 4. Database dependency

The configured database name is `CinemaSystem`.

The repository contains code that accesses tables including `Hall`, `Movie`, `Timing`, and `Order`, and code that calls stored procedures such as `CheckCustomerLogin`.

The current repository does not contain:

- SQL table definitions
- stored procedure definitions
- seed data
- a SQL Server backup

Because those assets are missing, the exact database constraints, indexes, triggers, and stored-procedure behavior cannot be verified from this repository.

## 5. Data access patterns

Database access appears directly in multiple WinForms classes through `SqlConnection`, `SqlCommand`, `SqlDataAdapter`, `DataSet`, and `SqlDataReader`.

Some commands use stored procedures with parameters. Other SQL statements are constructed through string concatenation.

Connection creation, opening, closing, and exception handling are implemented separately in multiple forms.

## 6. Ticket booking flow

`BuyTickets.cs`:

1. reads hall row / column information;
2. creates seat controls dynamically;
3. loads sold seats from the `Order` table;
4. marks selected seats in the UI;
5. checks the selected seats again before insertion;
6. inserts order records.

The seat check and order insertion are separate database operations in the application code.

Whether the database itself also prevents duplicate booking cannot be determined because the database schema and constraints are not available.

## 7. Configuration

The original repository contained a SQL Server connection string in `app.config`.

The current branch uses a Windows Integrated Authentication example instead:

```text
Data Source=.;Initial Catalog=CinemaSystem;Integrated Security=True
```

The old value remains part of Git history unless that history is rewritten.

## 8. Tests

No automated test project is present in the current repository.

Runtime behavior that depends on the missing database cannot be reproduced from the repository alone.
