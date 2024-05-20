namespace Kiosk.Abstractions.Models.Pagination;

using Kiosk.Abstractions.Models.FilterAndSorting;

public class PaginationRequest : FilterAndSorting
{
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 30;
}