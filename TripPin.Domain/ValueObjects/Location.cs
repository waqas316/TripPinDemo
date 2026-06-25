namespace TripPin.Domain.ValueObjects;

public sealed record Location(string Address, City? City)
{
    public override string ToString() => City is null ? Address : $"{Address}, {City}";
}
