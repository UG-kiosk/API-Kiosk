using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kiosk.Abstractions.Models;

public class EctsSubjectRequest : BaseEctsSubjectRequest
{
    public required int Year { get; set; }
    
}