using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace RaceDay.Api;

// Database-free demonstration store. Data exists only while the API process is running.
public sealed class InMemoryStore
{
    public ConcurrentDictionary<Guid, User> Users { get; } = new();
    public ConcurrentDictionary<Guid, RaceEvent> Events { get; } = new();
    public ConcurrentDictionary<Guid, EventCategory> Categories { get; } = new();
    public ConcurrentDictionary<Guid, Enrolment> Enrolments { get; } = new();
    public ConcurrentDictionary<Guid, RaceResult> Results { get; } = new();
    public PasswordHasher<User> Hasher { get; } = new();
    public User? FindByEmail(string email) => Users.Values.FirstOrDefault(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
}
