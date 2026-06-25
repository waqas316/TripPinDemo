using System.Net;
using System.Text;
using TripPin.Domain.Enums;
using TripPin.Infrastructure.Odata;
using TripPin.Infrastructure.Odata.Dtos;
using Xunit;

namespace TripPin.Tests;

public class PeopleTests
{
    private readonly ODataQueryBuilder _builder = new();

    [Fact]
    public async Task GetAll_Returns_All()
    {
        var firstPage = """
        { "value": [ { "UserName": "a", "FirstName": "Anna" } ],
          "@odata.nextLink": "https://services.odata.org/V4/TripPinServiceRW/People?$skiptoken=1" }
        """;
        var secondPage = """{ "value": [ { "UserName": "b", "FirstName": "Ben" } ] }""";
        var handler = StubHttpMessageHandler .Sequence(firstPage, secondPage);

        var people = await CreateRepository(handler).GetAllAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, people.Count);
        Assert.Equal(["a", "b"], people.Select(p => p.UserName));
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task SearchAsync_With_Filter()
    {
        var handler = StubHttpMessageHandler .Sequence("""{ "value": [] }""");
        var repo = CreateRepository(handler);

        await repo.SearchAsync("Russell", TestContext.Current.CancellationToken);

        var requested = Uri.UnescapeDataString(handler.LastRequestUri!.ToString());
        Assert.Contains("contains(tolower(UserName),'russell')", requested);
        Assert.Contains("contains(tolower(FirstName),'russell')", requested);
        Assert.Contains("contains(tolower(LastName),'russell')", requested);
    }


    [Fact]
    public void SearchQuery_Filters_With_CaseInsensitively()
    {
        var uri = Uri.UnescapeDataString(_builder.BuildPeopleQuery("Russell"));

        Assert.Contains("contains(tolower(UserName),'russell')", uri);
        Assert.Contains("contains(tolower(FirstName),'russell')", uri);
        Assert.Contains("contains(tolower(LastName),'russell')", uri);
        Assert.Contains(" or ", uri);
    }

    [Fact]
    public async Task SearchAsync_With_Empty_Filter_Returns_All()
    {
        var handler = StubHttpMessageHandler .Sequence("""{ "value": [ { "UserName": "a" } ] }""");
        var repo = CreateRepository(handler);

        await repo.SearchAsync("   ", TestContext.Current.CancellationToken);

        var requested = Uri.UnescapeDataString(handler.LastRequestUri!.ToString());
        Assert.DoesNotContain("$filter", requested);
    }

    [Fact]
    public async Task GetByUserName_Returns_Null_on_404()
    {
        var person = await CreateRepository(StubHttpMessageHandler .Status(HttpStatusCode.NotFound))
            .GetDetailsAsync("nobody", TestContext.Current.CancellationToken);
        Assert.Null(person);
    }


    [Fact]
    public async Task GetByUserName_Maps_Correctly_To_Person()
    {
        const string json = """
        {
          "UserName": "russellwhyte", "FirstName": "Russell", "LastName": "Whyte", "Gender": "Male",
          "Emails": ["Russell@example.com"],
          "AddressInfo": [{ "Address": "187 Suffolk Ln.", "City": { "Name": "Boise", "Region": "ID", "CountryRegion": "United States" } }]
        }
        """;
        var person = await CreateRepository(StubHttpMessageHandler .Sequence(json))
            .GetDetailsAsync("russellwhyte", TestContext.Current.CancellationToken);

        Assert.NotNull(person);
        Assert.Equal("Russell Whyte", person!.FullName);
        Assert.Equal("Russell@example.com", Assert.Single(person.Emails));
        Assert.Equal("Boise", Assert.Single(person.Addresses).City!.Name);
    }

    [Fact]
    public void BuildPersonByKeyQuery_With_Empty_Key_Throws()
    {
        Assert.Throws<ArgumentException>(() => _builder.BuildPersonByKeyQuery("  "));
    }

    [Fact]
    public void Mapper_Maps_All_Fields_Correctly()
    {
        var dto = new PersonDto
        {
            UserName = "russellwhyte",
            FirstName = "Russell",
            LastName = "Whyte",
            Gender = "Male",
            Emails = ["russell@example.com"],
            AddressInfo =
            [
                new LocationDto
                {
                    Address = "187 Suffolk Ln.",
                    City = new CityDto { Name = "Boise", Region = "ID", CountryRegion = "United States" }
                }
            ]
        };

        var person = PersonMapper.ToDomain(dto);

        Assert.Equal("russellwhyte", person.UserName);
        Assert.Equal("Russell Whyte", person.FullName);
        Assert.Equal(Gender.Male, person.Gender);
        Assert.Equal("russell@example.com", Assert.Single(person.Emails));
        Assert.Equal("Boise", Assert.Single(person.Addresses).City!.Name);
    }

    //===============
    // helpers
    //===============
    private ODataPeopleRepository CreateRepository(StubHttpMessageHandler  handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://services.odata.org/TripPinRESTierService/") };
        return new ODataPeopleRepository(client, _builder);
    }

    private sealed class StubHttpMessageHandler  : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;

        private StubHttpMessageHandler (IEnumerable<HttpResponseMessage> responses)
            => _responses = new Queue<HttpResponseMessage>(responses);

        public int RequestCount { get; private set; }

        public Uri? LastRequestUri { get; private set; }

        public static StubHttpMessageHandler  Sequence(params string[] jsonBodies)
            => new(jsonBodies.Select(body => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            }));

        public static StubHttpMessageHandler  Status(HttpStatusCode status)
            => new([new HttpResponseMessage(status)]);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            LastRequestUri = request.RequestUri;
            return Task.FromResult(_responses.Dequeue());
        }
    }
}