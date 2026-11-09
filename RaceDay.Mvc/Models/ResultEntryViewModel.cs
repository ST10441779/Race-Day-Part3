using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class ResultEntryViewModel
{
    public Guid EventId { get; set; }
    public Guid EnrolmentId { get; set; }
    public Guid ParticipantId { get; set; }
    public string CategoryName { get; set; } = "";

    [Required(ErrorMessage = "Enter the finishing time.")]
    [RegularExpression(@"^\d{1,2}:[0-5]\d:[0-5]\d$",
        ErrorMessage = "Use the format HH:MM:SS, for example 00:45:32.")]
    public string FinishTime { get; set; } = "";

    [Required]
    [Range(1, 100000, ErrorMessage = "Position must be at least 1.")]
    public int Position { get; set; }
}

public class RaceResultViewModel
{
    public Guid Id { get; set; }
    public Guid EnrolmentId { get; set; }
    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
}

public class ParticipantResultViewModel
{
    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public string CategoryName { get; set; } = "";
    public TimeSpan FinishTime { get; set; }
    public int Position { get; set; }
}