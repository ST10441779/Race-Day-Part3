namespace RaceDay.Mvc.Models;

public class EventDetailsViewModel
{
    public RaceEventViewModel Event { get; set; } = new();

    public List<CategoryViewModel> Categories { get; set; } = new();
}