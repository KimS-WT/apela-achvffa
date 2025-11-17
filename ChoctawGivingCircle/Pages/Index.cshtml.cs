using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChoctawGivingCircle.Pages;

public class IndexModel : PageModel
{
    public List<RequestCard> SampleRequests { get; private set; } = [];
    public List<InfoCard> SidebarCards { get; private set; } = [];

    public record RequestCard(string Title, string Category, DateTime CreatedOn, string Status);
    public record InfoCard(string Title, string Body, string AccentClass);

    public void OnGet()
    {
        SampleRequests =
        [
            new RequestCard("STEM Textbooks", "Education", DateTime.UtcNow.AddDays(-3), "Open"),
            new RequestCard("Regalia Materials", "Regalia", DateTime.UtcNow.AddDays(-10), "Approved"),
            new RequestCard("Elder Care Travel", "Health", DateTime.UtcNow.AddDays(-20), "Submitted")
        ];

        SidebarCards =
        [
            new InfoCard("Community Resources", "Download planning templates, grant tips, and partner contacts.", "accent-gold"),
            new InfoCard("Donations", "Track general fund balance and assigned gifts in real time.", "accent-red"),
            new InfoCard("Stories", "Read how the Choctaw Giving Circle is supporting caregivers.", "accent-orange")
        ];
    }
}
