using System.ComponentModel.DataAnnotations;

namespace Kiosk.Abstractions.Models;

public class Pagination
{
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int Page { get; set; } = 1;
    
    public int TotalPages { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Only positive number allowed below 100")]
    public int ItemsPerPage { get; set; }
    
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