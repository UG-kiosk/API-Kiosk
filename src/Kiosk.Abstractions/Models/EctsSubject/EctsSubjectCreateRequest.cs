using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kiosk.Abstractions.Enums;

namespace Kiosk.Abstractions.Models;

public class EctsSubjectCreateRequest : SubjectResponse
{
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Degree Degree { get; set; }

    [Range(1, 6)] public required int Term { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Range(1, 3)] public required Year Year { get; set; }

    public required List<int> RecruitmentYear { get; set; }
}