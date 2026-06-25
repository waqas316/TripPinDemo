using TripPin.Domain.Enums;
using TripPin.Domain.ValueObjects;

namespace TripPin.Domain.Entities;

public sealed class Person
{
    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }

    public List<string> Emails { get; set; }

    public List<Location> Addresses { get; set; }

    public string FullName => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";

    public Person(
       string userName,
       string firstName,
       string? lastName,
       Gender gender,
       List<string>? emails = null,
       List<Location>? addresses = null)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName ?? "";
        Gender = gender;
        Emails = emails ?? new();
        Addresses = addresses ?? new();
    }
}
