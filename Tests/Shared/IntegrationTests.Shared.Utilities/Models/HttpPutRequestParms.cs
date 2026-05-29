using System.Net;
using Shared.Models;

namespace IntegrationTests.Shared.Models;

public record HttpPutRequestParms
{
    public HttpPutRequestParms()
    {
        QueryStringParms = new BaseServiceGet { DeleteCache = true };
    }
    public HttpClient Client { get; set; }
    public string ApiEndPoint { get; set; }
    public int RecordId { get; set; }
    public object RequestObject { get; set; }
    public string Token { get; set; }
    public BaseServiceGet QueryStringParms { get; set; }
    public HttpStatusCode ExpectedStatusCode { get; set; } = HttpStatusCode.OK;
}