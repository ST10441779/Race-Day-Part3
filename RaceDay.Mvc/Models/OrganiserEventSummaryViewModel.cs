namespace RaceDay.Mvc.Models;

public class OrganiserEventSummaryViewModel
{
    public RaceEventViewModel Event { get; set; } = new();

    public int TotalEnrolments { get; set; }
}

public class OrganiserDashboardViewModel
{
    public int TotalEvents { get; set; }

    public int UpcomingEvents { get; set; }

    public int PastEvents { get; set; }

    public int TotalEnrolments { get; set; }

    public List<OrganiserEventSummaryViewModel> Events { get; set; }
        = new();
}