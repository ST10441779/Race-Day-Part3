namespace RaceDay.Mvc.Models;

public class CategoryViewModel
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; } = "";
    public string CategoryType { get; set; } = "";
}