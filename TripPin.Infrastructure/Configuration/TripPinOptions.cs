using System.ComponentModel.DataAnnotations;

namespace TripPin.Infrastructure.Configuration;

public sealed class TripPinOptions
{
    public const string SectionName = "TripPin";

    [Required]
    public string BaseAddress { get; set; } = "https://services.odata.org/TripPinRESTierService/";

    [Range(1, 300)]
    public int TimeoutSeconds { get; set; } = 30;
}
