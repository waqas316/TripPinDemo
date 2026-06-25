using System.Text.Json.Serialization;

namespace TripPin.Infrastructure.Odata.Dtos;

public sealed class ODataCollection<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = [];

    [JsonPropertyName("@odata.nextLink")]
    public string? NextLink { get; set; }
}

public sealed class PersonDto
{
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public List<string>? Emails { get; set; }
    public List<LocationDto>? AddressInfo { get; set; }
}

public sealed class LocationDto
{
    public string? Address { get; set; }
    public CityDto? City { get; set; }
}

public sealed class CityDto
{
    public string? Name { get; set; }
    public string? CountryRegion { get; set; }
    public string? Region { get; set; }
}
