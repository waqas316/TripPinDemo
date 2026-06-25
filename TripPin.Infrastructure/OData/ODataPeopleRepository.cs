using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TripPin.Domain.Entities;
using TripPin.Domain.People;
using TripPin.Infrastructure.Odata.Dtos;

namespace TripPin.Infrastructure.Odata;

public class ODataPeopleRepository(HttpClient httpClient, ODataQueryBuilder queryBuilder) : IPeopleRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const int MaxPagesToFollow = 100;

    public Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return GetPeopleAsync(queryBuilder.BuildPeopleQuery(), cancellationToken);
    }

    public Task<List<Person>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetAllAsync(cancellationToken);

        return GetPeopleAsync(queryBuilder.BuildPeopleQuery(searchTerm), cancellationToken);
    }

    public async Task<Person?> GetDetailsAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        var requestUri = queryBuilder.BuildPersonByKeyQuery(userName);

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        EnsureSuccess(response, requestUri);

        var dto = await response.Content.ReadFromJsonAsync<PersonDto>(JsonOptions, cancellationToken);
        return dto is null ? null : PersonMapper.ToDomain(dto);
    }

    private async Task<List<Person>> GetPeopleAsync(
        string requestUri, CancellationToken cancellationToken)
    {
        var people = new List<Person>();
        string? next = requestUri;

        for (var page = 0; next is not null && page < MaxPagesToFollow; page++)
        {
            using var response = await httpClient.GetAsync(next, cancellationToken);
            EnsureSuccess(response, next);

            var payload = await response.Content.ReadFromJsonAsync<ODataCollection<PersonDto>>(JsonOptions, cancellationToken);

            people.AddRange((payload?.Value ?? []).Select(PersonMapper.ToDomain));
            next = payload?.NextLink;
        }

        return people;
    }

    private static void EnsureSuccess(HttpResponseMessage response, string requestUri)
    {
        if (response.IsSuccessStatusCode)
            return;

        throw new ODataRequestException($"OData request '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}).", response.StatusCode);
    }
}
