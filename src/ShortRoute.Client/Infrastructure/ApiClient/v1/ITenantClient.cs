using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Commands.Authentication.Tenants;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Responses.Authentication.Tenants;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface ITenantsClient : IApiClient
{
    /// <summary>
    /// Gets a list of all the tenants
    /// </summary>
    [Get("/api/v1/tenants")]
    public Task<GetTenantsResponse> TenantsGetList(string? pagination = null, string? filter = null, string? sort = null);

    /// <summary>
    /// Creates a tenant
    /// </summary>
    [Post("/api/v1/tenants")]
    public Task TenantsCreate(CreateTenantCommand createTenant);

    /// <summary>
    /// Updates a tenant
    /// </summary>
    [Put("/api/v1/tenants")]
    public Task TenantsUpdate(UpdateTenantCommand updateTenant);

    /// <summary>
    /// Changes the tenant to disabled or active
    /// </summary>
    [Post("/api/v1/tenants/active")]
    public Task TenantsChangeActive(ChangeTenantActiveCommand changeTenantActive);

    /// <summary>
    /// Gets a list of a single tenant by id
    /// </summary>
    [Get("/api/v1/tenants/{id}")]
    public Task<TenantDto> TenantsGetSingle(string id);

    /// <summary>
    /// Deletes a tenant by id
    /// </summary>
    [Delete("/api/v1/tenants/{id}")]
    public Task TenantsDelete(string id);

    /// <summary>
    /// Gets all the tenant specific roles
    /// </summary>
    [Get("/api/v1/tenants/roles")]
    public Task<GetTenantsRolesResponse> TenantsRoles();
}
