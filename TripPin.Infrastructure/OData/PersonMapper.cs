using TripPin.Domain.Entities;
using TripPin.Domain.Enums;
using TripPin.Domain.ValueObjects;
using TripPin.Infrastructure.Odata.Dtos;

namespace TripPin.Infrastructure.Odata;

public class PersonMapper
{
    public static Person ToDomain(PersonDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Person(
            userName: dto.UserName ?? throw new InvalidOperationException("Person is missing its UserName key."),
            firstName: dto.FirstName ?? string.Empty,
            lastName: dto.LastName ?? string.Empty,
            gender: ParseGender(dto.Gender),
            emails: ToEmails(dto.Emails),
            addresses: ToAddresses(dto.AddressInfo));
    }

    private static Gender ParseGender(string? value) => Enum.TryParse<Gender>(value, ignoreCase: true, out var gender) ? gender : Gender.Unknown;

    private static List<string> ToEmails(List<string>? emails) => emails?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? [];

    private static List<Location> ToAddresses(List<LocationDto>? addresses)
    {
        return addresses?
                .Where(address => address is not null && !string.IsNullOrWhiteSpace(address.Address))
                .Select(x => new Location(x.Address!, ToCity(x.City)))
                .ToList() ?? [];
    }

    private static City? ToCity(CityDto? city)
    {
        return city is null
                 ? null
                 : new City(city.Name ?? string.Empty, city.CountryRegion ?? string.Empty, city.Region ?? string.Empty);
    }
}
