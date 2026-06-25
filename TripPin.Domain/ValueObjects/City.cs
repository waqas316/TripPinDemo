namespace TripPin.Domain.ValueObjects;

public sealed record City(string Name, string CountryRegion, string Region)
{
    public override string ToString()
    {
        var parts = new[] { Name, Region, CountryRegion }.Where(p => !string.IsNullOrWhiteSpace(p));

        return string.Join(", ", parts);
    }
}
