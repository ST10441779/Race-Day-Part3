namespace RaceDay.Mvc.Models;

public class EnrolmentViewModel
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public Guid EventId { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime EnrolledAt { get; set; }
}