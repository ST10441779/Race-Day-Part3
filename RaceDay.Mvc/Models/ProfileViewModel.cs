using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = "";

    public string Role { get; set; } = "";
}