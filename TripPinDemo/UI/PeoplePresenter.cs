using System.Text;
using TripPin.Domain.Entities;

namespace TripPin.Console.UI;

public static class PeoplePresenter
{
    public static string RenderList(IReadOnlyList<Person> people)
    {
        if (people?.Count == 0)
            return "No people found.";

        var sb = new StringBuilder();
        sb.AppendLine($"{"#",-3} {"Username",-16} {"Name",-20} {"Gender",-7} Email(s)");
        sb.AppendLine(new string('-', 78));

        for (var i = 0; i < people.Count; i++)
        {
            var p = people[i];
            sb.AppendLine(
                $"{i + 1,-3} {Truncate(p.UserName, 16),-16} {Truncate(p.FullName, 20),-20} " +
                $"{p.Gender,-7} {Truncate(EmailSummary(p), 32)}");
        }

        sb.AppendLine(new string('-', 78));
        sb.Append($"{people.Count} {(people.Count == 1 ? "person" : "people")}.");
        return sb.ToString();
    }

    public static string RenderDetails(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        var sb = new StringBuilder();
        sb.AppendLine(new string('=', 60));
        sb.AppendLine($"  {person.FullName}  (@{person.UserName})");
        sb.AppendLine(new string('=', 60));
        sb.AppendLine($"Gender : {person.Gender}");
        sb.AppendLine($"Emails : {(person.Emails.Count > 0 ? string.Join(", ", person.Emails) : "—")}");

        sb.AppendLine();
        sb.AppendLine($"Addresses ({person.Addresses.Count}):");
        if (person.Addresses.Count == 0)
            sb.AppendLine("  —");
        else
        {
            foreach (var address in person.Addresses)
                sb.AppendLine($"  • {address}");
        }

        return sb.ToString();
    }

    private static string EmailSummary(Person person)  => person.Emails.Count > 0 ? string.Join(", ", person.Emails) : "—";

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value[..(max - 1)] + "…";
    }

}
