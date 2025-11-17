# Choctaw Giving Circle

A demo ASP.NET Core 9.0 Razor Pages application implementing a small mutual-aid platform. It models requesters (community members), donors/supporters, and admins who approve and allocate resources. The project is intentionally lightweight and structured for demos, teaching, and iterative feature development.

## Quick highlights
- Requesters submit needs (AssistanceRequest) that carry Category, Priority, EstimatedCost, and a Status workflow.
- Donors create Contributions toward a Request or to the General Fund.
- Admins review requests, allocate general-fund contributions, and perform a co-signature approval flow for sensitive allocations.
- Fake email delivery is used for demos and writes to an `EmailLogs` table for audit and verification.

## Screenshots
<p align="center">
  <img src="assets/screenshots/home.png" alt="Admin Home" width="260">
  <img src="assets/screenshots/admin-all-donations.png" alt="All Donations" width="260">
  <img src="assets/screenshots/pending-fund-approvals.png" alt="Pending Fund Approvals" width="260">
  <img src="assets/screenshots/admin-approved-requests.png" alt="Admin Approved Requests" width="260">
</p>

## Project layout
- `ChoctawGivingCircle/`  main web app and Razor Pages.
- `ChoctawGivingCircle/Data/`  EF Core `ApplicationDbContext`, `SeedData`, and Identity extensions.
- `ChoctawGivingCircle/Models/`  domain entities (AssistanceRequest, Contribution, EmailLog, FundAllocationApproval, DonationDropOff, Location).
- `ChoctawGivingCircle/Pages/`  Razor Pages for public flows and admin UIs (Requests, Contributions, Admin/*).
- `ChoctawGivingCircle.Tests/`  xUnit tests that exercise allocation and approval logic.
- `tools/PrintEmailLogs/`  small console tool to inspect persisted `EmailLogs` (useful for demos).

## Prerequisites
- .NET 9 SDK installed.
- (Optional) EF Core tools for migrations: `dotnet tool install --global dotnet-ef`.

## Local setup & run
1. Restore and build:

```powershell
dotnet restore
dotnet build ./ChoctawGivingCircle/ChoctawGivingCircle.csproj
```

2. Run the app (development):

```powershell
dotnet run --project ./ChoctawGivingCircle/ChoctawGivingCircle.csproj
```

3. Open the printed URL (e.g., `https://localhost:5001` or `http://localhost:5273`). The app will create or update the SQLite DB automatically at startup when migrations are applied.

## Database / Migrations
- Migrations live in `ChoctawGivingCircle/Migrations/`.
- To add a migration from the repo root:

```powershell
dotnet ef migrations add <Name> --project ./ChoctawGivingCircle/ChoctawGivingCircle.csproj
dotnet ef database update --project ./ChoctawGivingCircle/ChoctawGivingCircle.csproj
```

## Seed data & demo accounts
The app seeds demo users and data on first run. Common seeded accounts used in demos:

```
### Sample Accounts (seed data)

- `admin@demo.local` / `Admin!123`
- `admin.supporter@demo.local` / `AdminSupport!123`
- `admin.requester@demo.local` / `AdminRequest!123`
- `supporter1@demo.local` / `Supporter!123`
- `supporter2@demo.local` / `Supporter!123`
- `requester1@demo.local` / `Requester!123`
- `requester2@demo.local` / `Requester!123`

```

## Core demo flows (recommended)
1. Sign in as a Requester and create an Assistance Request (Title, Description, EstimatedCost, Category).
2. Sign in as Admin and review the Submitted request  Approve it (status moves to `Approved/Open`).
3. As Donor, visit the Request details and click Donate (or donate to General Fund).
4. When a General Fund gift is created, Admins allocate it to a request via `Admin  Contributions  Allocate`. Large or self-allocated amounts may enter a pending approval (co-sign) queue at `Admin  Contributions  Approvals`.
5. Use `tools/PrintEmailLogs` or the `EmailLogs` table to verify fake emails were recorded for allocation/approval notifications.

## Running tests

```powershell
dotnet test ./ChoctawGivingCircle.Tests/ChoctawGivingCircle.Tests.csproj
```

## Inspecting email logs (demo)
The demo uses `FakeEmailService` which writes a record to the `EmailLogs` table instead of sending real email. To inspect recent logs run the included tool:

```powershell
cd tools/PrintEmailLogs
dotnet run
```

## Troubleshooting
- Locked build artifacts: Stop any running `dotnet run` processes if `dotnet build` fails with a file lock (Windows example):

```powershell
Stop-Process -Id <PID> -Force
```

- Validation on server forms (e.g., `UserId` required): Ensure you're signed in. The app now assigns `UserId` server-side for request creation to avoid client-side tampering and validation ordering problems.

## Developer notes
- Business rules and workflow logic belong in `Services/` (e.g., `AssistanceRequestService`, `ContributionService`). Keep PageModel methods thin and delegate to services.
- The `FundAllocationApproval` model and the Approvals UI implement a co-signature flow to ensure two admins sign off on sensitive allocations.

## Further reading and demos
- `DEMO_GUIDE.md` contains an ai-generated step-by-step developer demo and sample commands used during development (allocation + approval flow examples). (Coming Soon/In Development)

## Contributing & license
- This repo is a demo; if you plan to open-source it publicly, add a LICENSE and update the README accordingly. Contributions are welcome  open issues or PRs to propose changes.

## Maintainer
- `KimS-WT` (repo owner / demo maintainer)

--This README.md was generated using AI
