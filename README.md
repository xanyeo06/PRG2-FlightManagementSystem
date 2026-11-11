# PRG2 — Changi Airport T5 Flight Management System

A C# console application that simulates terminal operations for a fictional Terminal 5: it loads airlines, flights and gates from CSV files, assigns flights to gates and bills each airline for the day. Written for the Programming 2 module at Ngee Ann Polytechnic, it demonstrates inheritance, dictionary-based composition, `IComparable` sorting and file parsing. It is an academic simulation with no affiliation to Changi Airport or any real operator.

## Contents

| File | Description |
| --- | --- |
| `PRG2-FlightManagementSystem/Program.cs` | Top-level statements: CSV loading, menu loop, all nine features |
| `PRG2-FlightManagementSystem/Terminal.cs` | Aggregate root: airlines, flights, gates and gate fees |
| `PRG2-FlightManagementSystem/Airline.cs` | Airline identity, its flight dictionary, fee and discount calculation |
| `PRG2-FlightManagementSystem/Flight.cs` | Base flight entity; orders by expected time |
| `PRG2-FlightManagementSystem/BoardingGate.cs` | Gate identity, three capability flags, assigned flight |
| `PRG2-FlightManagementSystem/NORMFlight.cs` | No request code; arrival/departure fee only |
| `PRG2-FlightManagementSystem/DDJBFlight.cs` | DDJB request; adds 300 |
| `PRG2-FlightManagementSystem/CFFTFlight.cs` | CFFT request; adds 150 |
| `PRG2-FlightManagementSystem/LWTTFlight.cs` | LWTT request; adds 500 |
| `PRG2-FlightManagementSystem/airlines.csv` | Eight airlines: name and two-letter code |
| `PRG2-FlightManagementSystem/boardinggates.csv` | 66 gates with DDJB, CFFT and LWTT flags |
| `PRG2-FlightManagementSystem/flights.csv` | 30 seed flights with request codes |

## Running

```bash
dotnet run --project PRG2-FlightManagementSystem
```

Target framework is `net8.0`. The CSVs are opened by bare filename, so the process must start in their directory (the build output, where the `.csproj` copies them).

## Domain model

`Terminal` owns four dictionaries: airlines by code, flights by number, gates by name, and a gate-fee table. Each `Airline` keeps a second dictionary of the same flight objects, so a flight is reachable from both. A `BoardingGate` holds at most one `Flight`; special-request flights are subclasses of `Flight`.

```
Terminal ── Airlines, Flights, BoardingGates, GateFees
Flight (base) ── NORMFlight | DDJBFlight (+300) | CFFTFlight (+150) | LWTTFlight (+500)
BoardingGate ──holds 0..1──> Flight
```

| Class | Responsibility |
| --- | --- |
| `Terminal` | Owns the four collections; resolves a flight's airline |
| `Airline` | Adds and removes flights; computes subtotal and discount |
| `Flight` | Flight number, route, expected time, status; comparable by time |
| `BoardingGate` | Gate name, three capability flags, current flight |

## Features

| Option | Feature |
| --- | --- |
| 1 | List all flights with airline name, route and expected time |
| 2 | List all gates and their DDJB, CFFT and LWTT support |
| 3 | Assign a gate to a flight, then optionally change its status |
| 4 | Create a flight, validate it and append it to `flights.csv` |
| 5 | Display all flights for a chosen airline code |
| 6 | Modify flight details or delete a flight |
| 7 | Display the schedule sorted by expected time |
| 8 | Total fees per airline and terminal totals (feature b); refuses to run until every flight has a gate |
| 9 | Assign every unassigned flight to the first free compatible gate (feature a) |
| 0 | Exit |

## Fee calculation

Fees are per flight. Arriving means destination `Singapore (SIN)`; departing means origin `Singapore (SIN)`. The discounts below are summed and subtracted from the subtotal.

| Component | Amount |
| --- | --- |
| Arriving / departing flight | 500 / 800 |
| Boarding gate, departing only | +300 |
| DDJB / CFFT / LWTT surcharge | +300 / +150 / +500 |

| Rule | Condition | Discount |
| --- | --- | --- |
| Volume | More than 5 flights | 3% of subtotal |
| Off-peak | Time at or before 11:00, or at or after 21:00 | 110 per flight |
| Preferred origin | Origin DXB, BKK or NRT | 25 per flight |
| No request code | No DDJB, CFFT or LWTT code | 50 per flight |
| Groups of three | Each complete group of three flights | 350 per group |

For `SQ` in the seed data: SQ 115 (NRT–SIN, 11:45, DDJB) 800, SQ 221 (MNL–SIN, 08:50, DDJB) 800, SQ 687 (SIN–SYD, 14:25, LWTT) 1600 and SQ 512 (MNL–SIN, 08:35, CFFT) 650 give a subtotal of 3850; discounts of 220 (two off-peak), 25 (one preferred origin) and 350 (one group of three) total 595, leaving 3255.

## Implementation notes

- Loading skips the header and calls `Split(',')`, so quoted fields are unsupported. The request code is column 4; an empty value becomes `"N.A"` and selects `NORMFlight`. The airline comes from `flightNo.Split(' ')[0]`.
- The subclasses declare `CalculateFees()` without `override` and the base method is not `virtual`, so they shadow it and nothing calls them. The report uses `Airline.CalculateFees(flightSpecialRequestCodes)`, which re-derives each surcharge from a separate flight-number-to-code dictionary.
- Compatibility is checked only in the bulk path, by a `switch` on the code against `SupportsDDJB`, `SupportsCFFT` and `SupportsLWTT`; manual assignment checks existence and occupancy only. `Flight : IComparable<Flight>` compares `ExpectedTime` and option 7 applies `OrderBy(f => f)`; gate lookup is a linear scan of `BoardingGates`, repeated in each feature that needs it.

## Constraints and limitations

- Manual gate assignment ignores capability flags, so a DDJB flight can take a gate without DDJB support.
- A flight created without a request code is stored as `NONE`, which the loader has no branch for, so it is dropped on the next run; a number entered as `SQ115` also fails the airline lookup, which expects `SQ 115`.
- Parsing is culture-dependent: seed times and modifications use `Convert.ToDateTime`, creation uses `TryParseExact` with `dd/MM/yyyy HH:mm`, and only `TimeOfDay` affects fees. A short CSV row throws during loading, outside any `try` block, and ends the program.
- `Terminal.GateFees`, `Terminal.PrintAirlineFees`, `BoardingGate.CalculateFees` and the four subclass fee methods are unused; option 8 is labelled for one airline but reports all.

## Key takeaways

- The terminal aggregates dictionaries and each airline holds its own view of the same objects, which keeps lookup cheap but duplicates state that create and delete must keep consistent.
- The fee model exists twice, through subclasses and through a code-to-surcharge table, and only the table is exercised.
- `IComparable<Flight>` gives schedule ordering without a separate comparer, and validation sits in the console layer, so invariants hold only for input that passes those prompts.
