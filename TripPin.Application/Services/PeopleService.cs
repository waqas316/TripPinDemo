using TripPin.Application.Abstractions;
using TripPin.Domain.Entities;
using TripPin.Domain.People;

namespace TripPin.Application.Services;

public class PeopleService(IPeopleRepository repository) : IPeopleService
{
    public Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllAsync(cancellationToken);
    }

    public Task<List<Person>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return repository.SearchAsync(searchTerm?.Trim() ?? string.Empty, cancellationToken);
    }

    public async Task<Person?> GetDetailsAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        return await repository.GetDetailsAsync(userName.Trim(), cancellationToken);
    }
}
