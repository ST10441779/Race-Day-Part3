namespace RaceDay.Mvc.Models;

public class RaceEventViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Date { get; set; }
    public string Location { get; set; } = "";
    public double DistanceKm { get; set; }
    public string EventType { get; set; } = "";
    public Guid OrganiserId { get; set; }
}