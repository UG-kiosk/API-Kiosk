using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.FilterAndSorting;

public class FilterAndSorting
{
    public string? sortBy { get; set; }
    
    public Sorting? sortDirection { get; set; }
    
    public string? filterBy { get; set; }
    
    public string? filterValue { get; set; }
    
    public Degree? Degree { get; set; } = Enums.Degree.Bachelor;
}

