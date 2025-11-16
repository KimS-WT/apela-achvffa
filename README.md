# Choctaw Giving Circle

Choctaw Giving Circle is a demo Razor Pages web app that highlights a mutual-aid model inspired by Choctaw and broader Native community values. Tribal members (requesters) share specific needs, supporters (donors) browse those needs to offer funds, goods, or services, and admins oversee approvals plus fulfillment.

## Roles & Core Entities

- **Roles:** Requester, Donor, Admin (managed through ASP.NET Core Identity).
- **AssistanceRequest:** Captures the community member need, including category, priority, cost estimate, and status workflow (Draft ➜ Fulfilled).
- **Contribution:** Tracks monetary/item/service contributions tied to a request.
- **Tribe:** Lightweight entity to support future multi-tribe context for accounts.

## Running the Project

1. Restore dependencies:  
   `dotnet restore`
2. Apply any EF Core migrations (after you add them):  
   `dotnet ef database update`
3. Run the development server:  
   `dotnet run --project ChoctawGivingCircle/ChoctawGivingCircle.csproj`
4. Visit `https://localhost:5001` (or the console URL) to explore the app.

> Tip: Seed sample data and admin accounts via EF Core or custom initializers to make demos more compelling.
