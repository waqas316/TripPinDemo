using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TripPin.Application;
using TripPin.Console.UI;
using TripPin.Infrastructure;


var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services
    .AddTripPinApplication()
    .AddTripPinInfrastructure(builder.Configuration);

builder.Services.AddTransient<PeopleExplorer>();

using var host = builder.Build();

//Ctrl+C to to handle cancellation
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await using var scope = host.Services.CreateAsyncScope();
    var explorer = scope.ServiceProvider.GetRequiredService<PeopleExplorer>();
    await explorer.RunAsync(cts.Token);
    return 0;
}
catch (OperationCanceledException)
{
    Console.WriteLine();
    Console.WriteLine("Cancelled.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
    return 1;
}