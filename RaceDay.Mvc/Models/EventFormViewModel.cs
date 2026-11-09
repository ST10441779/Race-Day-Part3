using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class EventFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = "";

    [Required]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(30);

    [Required]
    public string Location { get; set; } = "";

    [Range(0.1, 500, ErrorMessage = "Enter a valid distance.")]
    public double DistanceKm { get; set; }

    [Required]
    public string EventType { get; set; } = "Run";
}