namespace Kiosk.Abstractions.Models;

public class Pagination
{ 
    public required int Page { get; set; }
    public int TotalPages { get; set; } = 1;
    public required int ItemsPerPage { get; set; }
    public bool HasNextPage { get; set; }
          
    public static int CalculateTotalPages(int totalRecords, int itemsPerPage)
    {
        return (totalRecords + itemsPerPage - 1) / itemsPerPage;
    }

    public static bool CalculateHasNextPage(int currentPage, int totalPages)
    {
        return currentPage < totalPages;
    }
}