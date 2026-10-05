# Project Notes

## 1. What this project is

This is a legacy undergraduate Windows Forms cinema booking system whose original GitHub snapshot dates to 2020.

The repository is valuable as a historical project and portfolio artifact, but it should be presented honestly: it demonstrates a substantial amount of end-to-end application logic, while also containing the kinds of architectural shortcuts that are common in early projects.

## 2. Recovered functional map

### Customer workflow

| Component | Responsibility |
| --- | --- |
| `Form1` | Customer login and entry point |
| `Form2` | Customer booking menu |
| `Form3` | Customer registration |
| `Form4` | Movie information and showtime selection |
| `BuyTickets` | Dynamic seat rendering, selection, availability check and order insertion |
| `Form5` | Order history and order operations |
| `Form6` | Membership / VIP workflow |

### Administrator workflow

| Component | Responsibility |
| --- | --- |
| `Form7` | Administrator login |
| `Form8` | Administrator navigation |
| `Form9` | Cinema hall management |
| `Form10` | Movie management |
| `Form11` | Showtime management |
| `Form12` | Order management |
| `Form13` | Customer management |
| `Form14` | Statistics navigation |
| `Form15` | Box-office statistics view |
| `Form17` | Occupancy statistics view |

`Form16` and `Form18` are retained as part of the historical source; their UI names are not self-describing enough to document more aggressively without reconstructing their full runtime flow.

## 3. Architecture recovered from the source

The checked-in project is the **WinForms UI layer** (`hh.csproj`).

The project originally referenced:

- `Model`
- `PublicLib`

as sibling projects outside this repository. Their source code is absent from the original upload, while compiled DLLs were present in the old `bin/Debug` directory.

The application also uses SQL Server directly from a number of forms through `System.Data.SqlClient`.

A simplified view is:

```text
WinForms UI (hh)
   ├── direct ADO.NET / SQL Server access
   ├── Model dependency
   ├── PublicLib dependency
   └── IrisSkin4 UI skin dependency
```

This means the codebase is not a clean layered architecture: presentation, business rules, and persistence responsibilities overlap.

## 4. Important technical debt

### SQL construction

Several queries are constructed by concatenating input into SQL strings. For example, movie-management and booking paths assemble SQL statements in application code.

A modernized version should use parameterized commands consistently.

### Booking concurrency

The booking flow first checks whether seats are already occupied and later inserts orders.

Conceptually:

```text
check seat availability
        ↓
user confirms / code continues
        ↓
insert order
```

Without a database transaction or uniqueness constraint protecting the whole operation, two concurrent clients can race between the check and insertion.

A production implementation should enforce seat uniqueness at the database level and perform the reservation in a transaction.

### Repeated data-access code

Connection creation, opening, exception handling and cleanup are repeated across many forms. A dedicated data-access layer would reduce duplication and make testing possible.

### UI/business coupling

Many form event handlers contain validation, SQL access, business rules and UI updates in the same method. A modern refactor would separate these responsibilities.

### Testing

No automated test suite is present. The existing code is primarily event-driven and database-coupled, which makes unit testing difficult without first extracting business logic.

## 5. Security cleanup

The original public `app.config` contained a SQL Server `sa` username and a hard-coded password.

The repository cleanup replaces that with Windows integrated authentication as a non-secret example.

Important: deleting a credential in a later commit does **not** erase it from Git history. If the old credential was ever used outside a disposable local development database, it should be considered exposed and rotated.

## 6. Suggested future restoration path

If this project is ever revived beyond portfolio preservation, the highest-value steps are:

1. Recover the original `Model` and `PublicLib` source projects if they still exist.
2. Recover/export the `CinemaSystem` SQL schema, stored procedures and minimal seed data.
3. Add a reproducible database setup script.
4. Replace concatenated SQL with parameterized commands.
5. Make ticket purchase transactional and enforce seat uniqueness in SQL Server.
6. Extract data access from WinForms event handlers.
7. Add a small integration-test suite around login, showtime selection and seat booking.
8. Only then consider a framework migration.

A framework rewrite should not be the first step: restoring reproducibility and correctness provides much more value than changing UI technology.
