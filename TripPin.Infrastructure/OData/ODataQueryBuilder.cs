namespace TripPin.Infrastructure.Odata;

public class ODataQueryBuilder
{
    private static readonly string[] SearchFields = ["UserName", "FirstName", "LastName"];

    private static string Option(string key, string value) => $"{key}={Uri.EscapeDataString(value)}";

    private static string EscapeStringLiteral(string value) => value.Replace("'", "''", StringComparison.Ordinal);

    public string BuildPeopleQuery(string? search = null)
    {
        var options = new List<string> { Option("$orderby", "UserName asc") };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var literal = EscapeStringLiteral(search.Trim().ToLowerInvariant());
            var clauses = SearchFields.Select(field => $"contains(tolower({field}),'{literal}')");
            options.Add(Option("$filter", string.Join(" or ", clauses)));
        }

        return "People?" + string.Join("&", options);
    }

    public string BuildPersonByKeyQuery(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is required.", nameof(userName));

        var key = Uri.EscapeDataString(EscapeStringLiteral(userName));
        return $"People('{key}')";
    }
}
