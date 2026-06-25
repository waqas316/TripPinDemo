using TripPin.Application.Abstractions;
using TripPin.Domain.Entities;
using SysConsole = System.Console;

namespace TripPin.Console.UI;

public sealed class PeopleExplorer (IPeopleService _people)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        SysConsole.WriteLine();
        SysConsole.WriteLine("===== TripPin Demo =====");

        while (!cancellationToken.IsCancellationRequested)
        {
            SysConsole.WriteLine();
            SysConsole.WriteLine("  1) List all people");
            SysConsole.WriteLine("  2) Search people by first name/last name or username");
            SysConsole.WriteLine("  3) View a person's details");
            SysConsole.WriteLine("  0) Exit");
            SysConsole.Write("Choose an option: ");

            switch (SysConsole.ReadLine()?.Trim())
            {
                case "1":
                    await ListPeopleAsync(cancellationToken);
                    break;
                case "2":
                    await SearchPeopleAsync(cancellationToken);
                    break;
                case "3":
                    await ShowDetailsAsync(cancellationToken);
                    break;
                case "0":
                case null:
                    SysConsole.WriteLine("Goodbye!");
                    return;
                default:
                    SysConsole.WriteLine("Unrecognised option, please try again.");
                    break;
            }
        }
    }

    private async Task ListPeopleAsync(CancellationToken cancellationToken)
    {
        var people = await TryGetPeopleAsync(() => _people.GetAllAsync(cancellationToken), "load people");
        if (people is not null)
            PrintList(people);
    }

    private async Task SearchPeopleAsync(CancellationToken cancellationToken)
    {
        SysConsole.Write("Name or username contains: ");
        var term = SysConsole.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(term))
        {
            SysConsole.WriteLine("No search term entered.");
            return;
        }

        var people = await TryGetPeopleAsync(() => _people.SearchAsync(term, cancellationToken), "search people");
        if (people is not null)
            PrintList(people);
    }

    private async Task ShowDetailsAsync(CancellationToken cancellationToken)
    {
        SysConsole.Write("Enter a username: ");
        var userName = SysConsole.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(userName))
        {
            SysConsole.WriteLine("No username entered.");
            return;
        }

        Person? person;
        try
        {
            person = await _people.GetDetailsAsync(userName, cancellationToken);
        }
        catch (Exception ex)
        {
            SysConsole.WriteLine($"Could not load '{userName}': {ex.Message}");
            return;
        }

        SysConsole.WriteLine();
        SysConsole.WriteLine(person is null
            ? $"No person found with username '{userName}'."
            : PeoplePresenter.RenderDetails(person));
    }

    private static void PrintList(IReadOnlyList<Person> people)
    {
        SysConsole.WriteLine();
        SysConsole.WriteLine(PeoplePresenter.RenderList(people));
    }

    private static async Task<List<Person>?> TryGetPeopleAsync(
        Func<Task<List<Person>>> operation, string what)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            SysConsole.WriteLine($"Could not {what}: {ex.Message}");
            return null;
        }
    }
}
