using Refit;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface ITenantClient
{
    /// <summary>
    /// Gets a list of all the tenants
    /// </summary>
    [Get("/api/v1/tenants")]
    public Task<ApiResponse<TenantDto[]>> TenantsGetList();

    /// <summary>
    /// Creates a tenant
    /// </summary>
    [Post("/api/v1/tenants")]
    public Task<ApiResponse<object>> TenantsCreate(TenantDto tenant);

    /// <summary>
    /// Updates a tenant
    /// </summary>
    [Put("/api/v1/tenants")]
    public Task<ApiResponse<object>> TenantsUpdate(TenantDto tenant);

    /// <summary>
    /// Gets a list of a single tenant by id
    /// </summary>
    [Get("/api/v1/tenants/{id}")]
    public Task<ApiResponse<TenantDto>> TenantsGetSingle();

    /// <summary>
    /// Deletes a tenant by id
    /// </summary>
    [Delete("/api/v1/tenants/{id}")]
    public Task<ApiResponse<object>> TenantsDelete();

    /// <summary>
    /// Gets all the tenant specific roles
    /// </summary>
    [Get("/api/v1/tenants/roles")]
    public Task<ApiResponse<string[]>> TenantRoles();
}
