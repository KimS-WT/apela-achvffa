# Choctaw Giving Circle

Choctaw Giving Circle is a demo Razor Pages web app that highlights a mutual-aid model inspired by Choctaw and broader Native community values. Tribal members (requesters) share specific needs, supporters (donors) browse those needs to offer funds, goods, or services, and admins oversee approvals plus fulfillment.

## Roles & Core Entities

- **Roles:** Requester, Donor, Admin (managed through ASP.NET Core Identity).
- **AssistanceRequest:** Captures the community member need, including category, priority, cost estimate, and status workflow (Draft ➜ Fulfilled).
- **Contribution:** Tracks monetary/item/service contributions tied to a request or the general donation fund (admins can allocate fund gifts to specific needs later).
- **Tribe:** Lightweight entity to support future multi-tribe context for accounts.

### Donation Flow

- Supporters may give directly to a specific request. Those contributions are immediately linked to the requester’s need.
- If a supporter doesn’t specify a requester, the gift lands in the General Fund. Admins allocate those contributions to active requests through the Contribution Management dashboard.
- Future roadmap: an AI-assisted matcher will recommend best-fit allocations (e.g., detecting that a donated desk matches a requester needing study furniture) and factor in feasibility and urgency.

### Sample Accounts (seed data)

- `admin@demo.local` / `Admin!123`
- `admin.supporter@demo.local` / `AdminSupport!123`
- `admin.requester@demo.local` / `AdminRequest!123`
- `supporter1@demo.local` / `Supporter!123`
- `supporter2@demo.local` / `Supporter!123`
- `requester1@demo.local` / `Requester!123`
- `requester2@demo.local` / `Requester!123`

## Running the Project

1. Restore dependencies:  
   `dotnet restore`
2. Apply any EF Core migrations (after you add them):  
   `dotnet ef database update`
3. Run the development server:  
   `dotnet run --project ChoctawGivingCircle/ChoctawGivingCircle.csproj`
4. Visit `https://localhost:5001` (or the console URL) to explore the app.

> Tip: Seed sample data and admin accounts via EF Core or custom initializers to make demos more compelling.
