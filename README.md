# Cinema Booking System

A legacy undergraduate cinema booking system built with **C# Windows Forms**, **.NET Framework 4.0**, and **SQL Server**.

> **Project status:** historical / portfolio project.  
> The original code was written as an undergraduate project and is preserved primarily for learning, review, and software-engineering archaeology rather than production use.

## Features

### Customer side

- Customer registration and login
- Movie and showtime browsing
- Dynamic seat selection
- Ticket booking and seat-availability checks
- Order history and refund-related operations
- Membership / VIP handling

### Administrator side

- Administrator login
- Cinema hall management
- Movie management
- Showtime management
- Order management
- Customer management
- Box-office and occupancy statistics

## Tech stack

- C#
- Windows Forms (WinForms)
- .NET Framework 4.0 Client Profile
- SQL Server
- ADO.NET / `System.Data.SqlClient`
- IrisSkin4 (legacy WinForms skin library)

The application targets **x86**.

## Repository status and reproducibility

This repository is a preserved legacy project, so there are several important limitations:

1. The UI project originally referenced two sibling source projects, `Model` and `PublicLib`, which were not included in the original GitHub upload.
2. This cleanup keeps the legacy compiled assemblies needed by the UI project under `lib/` so the original references can be preserved as far as possible.
3. The SQL Server database schema, seed data, and stored procedures are not currently included.
4. Because of the missing database assets, the project should **not** be described as clone-and-run reproducible yet.

## Configuration

The application expects a SQL Server database named `CinemaSystem`.

The committed `app.config` intentionally contains **no password**. Configure the connection string locally for your own environment.

Example using Windows authentication:

```xml
<add
  name="connStr"
  connectionString="Data Source=.;Initial Catalog=CinemaSystem;Integrated Security=True"
  providerName="System.Data.SqlClient" />
```

Do not commit real database credentials.

## Opening the project

A practical legacy setup is:

1. Use Windows.
2. Install a Visual Studio version capable of targeting .NET Framework 4.0.
3. Ensure SQL Server is available.
4. Restore or recreate the `CinemaSystem` database.
5. Open `hh.csproj`.
6. Adjust the local database connection string if required.

The historical project/namespace name `hh` is intentionally retained to avoid unnecessary source churn.

## Project layout

```text
.
├── Program.cs                 # Application entry point
├── Form1.cs                   # Customer login
├── Form2.cs                   # Customer menu
├── Form3.cs                   # Registration
├── Form4.cs                   # Movie / showtime selection
├── BuyTickets.cs              # Seat selection and booking
├── Form5.cs                   # Order history
├── Form6.cs                   # Membership handling
├── Form7.cs                   # Administrator login
├── Form8.cs                   # Administrator menu
├── Form9.cs                   # Hall management
├── Form10.cs                  # Movie management
├── Form11.cs                  # Showtime management
├── Form12.cs                  # Order management
├── Form13.cs                  # Customer management
├── Form14.cs                  # Statistics menu
├── Form15.cs / Form17.cs      # Statistics views
├── Properties/
├── lib/                       # Legacy binary dependencies
├── app.config
└── hh.csproj
```

## Engineering notes

This is useful as a snapshot of an early-stage desktop application, but several implementation choices should not be copied into a modern production system:

- UI, business logic, and data access are tightly coupled.
- Some SQL statements are assembled through string concatenation instead of parameterized commands.
- Seat availability is checked separately from order insertion, so concurrent purchases are not protected by a database transaction.
- Database connections and error handling are implemented repeatedly across forms.
- There is no automated test suite.
- The database schema and stored procedures are missing from the repository.

A more detailed technical review is available in [docs/PROJECT_NOTES.md](docs/PROJECT_NOTES.md).

## Historical note

The goal of the repository cleanup is **not** to disguise an undergraduate project as a modern production application. The original structure and coding style are intentionally visible; the repository-level documentation and hygiene have simply been improved so that another developer can understand what the project is, what it demonstrates, and what is still missing.

## License

No open-source license has been selected for this repository.
