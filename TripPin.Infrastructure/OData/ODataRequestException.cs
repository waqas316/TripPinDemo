using System.Net;

namespace TripPin.Infrastructure.Odata;

public sealed class ODataRequestException : Exception
{
    public ODataRequestException(string message, HttpStatusCode statusCode) : base(message) => StatusCode = statusCode;

    public HttpStatusCode StatusCode { get; }
}
