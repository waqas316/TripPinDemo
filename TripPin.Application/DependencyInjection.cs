using Microsoft.Extensions.DependencyInjection;
using TripPin.Application.Abstractions;
using TripPin.Application.Services;

namespace TripPin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTripPinApplication(this IServiceCollection services)
    {
        services.AddScoped<IPeopleService, PeopleService>();
        return services;
    }
}
