using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface ITenantClient : IApiClient
{
    /// <summary>
    /// Gets a list of all the tenants
    /// </summary>
    [Get("/api/v1/tenants")]
    public Task<TenantDto[]> TenantsGetList();

    /// <summary>
    /// Creates a tenant
    /// </summary>
    [Post("/api/v1/tenants")]
    public Task TenantsCreate(TenantDto tenant);

    /// <summary>
    /// Updates a tenant
    /// </summary>
    [Put("/api/v1/tenants")]
    public Task TenantsUpdate(TenantDto tenant);

    /// <summary>
    /// Gets a list of a single tenant by id
    /// </summary>
    [Get("/api/v1/tenants/{id}")]
    public Task<TenantDto> TenantsGetSingle();

    /// <summary>
    /// Deletes a tenant by id
    /// </summary>
    [Delete("/api/v1/tenants/{id}")]
    public Task TenantsDelete();

    /// <summary>
    /// Gets all the tenant specific roles
    /// </summary>
    [Get("/api/v1/tenants/roles")]
    public Task<string[]> TenantRoles();
}
