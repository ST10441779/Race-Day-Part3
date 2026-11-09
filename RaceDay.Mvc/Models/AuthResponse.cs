namespace RaceDay.Mvc.Models;

public class AuthResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";

    public string? Message { get; set; }
}