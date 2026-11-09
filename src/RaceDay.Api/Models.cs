namespace RaceDay.Api;

public static class Roles { public const string Organiser = "Organiser"; public const string Participant = "Participant"; }
public sealed class User { public Guid Id { get; set; } = Guid.NewGuid(); public string FullName { get; set; } = ""; public string Email { get; set; } = ""; public string Role { get; set; } = Roles.Participant; public string PasswordHash { get; set; } = ""; }
public sealed record RegisterRequest(string FullName, string Email, string Password, string Role);
public sealed record LoginRequest(string Email, string Password);
public sealed record ProfileUpdateRequest(string FullName, string Email);
public sealed class RaceEvent { public Guid Id { get; set; } = Guid.NewGuid(); public Guid OrganiserId { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; public DateTime Date { get; set; } public string Location { get; set; } = ""; public double DistanceKm { get; set; } public string EventType { get; set; } = "Run"; }
public sealed record EventRequest(string Name, string Description, DateTime Date, string Location, double DistanceKm, string EventType);
public sealed class EventCategory { public Guid Id { get; set; } = Guid.NewGuid(); public Guid EventId { get; set; } public string Name { get; set; } = ""; public string CategoryType { get; set; } = "Age"; }
public sealed record CategoryRequest(string Name, string CategoryType);
public sealed class Enrolment { public Guid Id { get; set; } = Guid.NewGuid(); public Guid ParticipantId { get; set; } public Guid EventId { get; set; } public Guid CategoryId { get; set; } public DateTime EnrolledAt { get; set; } = DateTime.UtcNow; }
public sealed record EnrolmentRequest(Guid EventId, Guid CategoryId);
public sealed class RaceResult { public Guid Id { get; set; } = Guid.NewGuid(); public Guid EnrolmentId { get; set; } public TimeSpan FinishTime { get; set; } public int Position { get; set; } }
public sealed record ResultRequest(Guid EnrolmentId, TimeSpan FinishTime, int Position);
