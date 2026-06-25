using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TripPin.Domain.People;
using TripPin.Infrastructure.Configuration;
using TripPin.Infrastructure.Odata;
namespace TripPin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTripPinInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TripPinOptions>()
            .Bind(configuration.GetSection(TripPinOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(o => Uri.TryCreate(o.BaseAddress, UriKind.Absolute, out _), "TripPin:BaseAddress must be an absolute URI.")
            .ValidateOnStart();

        services.AddSingleton<ODataQueryBuilder>();

        services.AddHttpClient<IPeopleRepository, ODataPeopleRepository>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<TripPinOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseAddress);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
