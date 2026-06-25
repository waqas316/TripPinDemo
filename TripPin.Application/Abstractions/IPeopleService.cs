using TripPin.Domain.Entities;

namespace TripPin.Application.Abstractions;

public interface IPeopleService
{
    Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Person>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<Person?> GetDetailsAsync(string userName, CancellationToken cancellationToken = default);
}
