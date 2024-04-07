using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models.Staff;

public class AcademicRequest
{
    public string Name { get; set; }
    public string Link { get; set; }
    public string Email { get; set; }
    public AcademicContent Content { get; set; }
}