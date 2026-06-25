using TripPin.Domain.Entities;

namespace TripPin.Domain.People;

public interface IPeopleRepository
{
    Task<List<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Person>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<Person?> GetDetailsAsync(string userName, CancellationToken cancellationToken = default);

}
