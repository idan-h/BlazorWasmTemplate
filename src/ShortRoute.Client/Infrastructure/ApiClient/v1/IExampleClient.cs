using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Endpoints;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IExampleClient : IApiClient
{
    [Get(Urls.Example)]
    Task<IEnumerable<string>> GetExamples();
}
