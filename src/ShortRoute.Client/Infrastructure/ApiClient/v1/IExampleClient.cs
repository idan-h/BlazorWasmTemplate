using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Dtos.Example;
using ShortRoute.Contracts.Endpoints;
using ShortRoute.Contracts.Responses.Example;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IExampleClient : IApiClient
{
    /// <summary>
    /// 
    /// </summary>
    [Get("/api/v1/example")]
    public Task<ExampleResponse> Example(string? pagination = null, string? filter = null, string? sort = null);

    /// <summary>
    /// 
    /// </summary>
    [Post("/api/v1/example/create")]
    public Task ExampleCreate();
}