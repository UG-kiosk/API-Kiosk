using System.ComponentModel.DataAnnotations;
using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Major;

public class FindMajorsRequest
{
    [Required(ErrorMessage = "Language is required.")]
    public Language Language { get; set; }
    public Degree? Degree { get; set; }
    public string? Name { get; set; }
}