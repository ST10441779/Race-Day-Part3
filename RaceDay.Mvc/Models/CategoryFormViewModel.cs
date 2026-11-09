using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class CategoryFormViewModel
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    [Required(ErrorMessage = "Enter a category name.")]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Enter a category type.")]
    [StringLength(50)]
    public string CategoryType { get; set; } = "Age";
}