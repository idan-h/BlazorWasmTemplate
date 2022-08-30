using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IRolesClient : IApiClient
{
    /// <summary>
    /// A list of all the roles the current user has permission to see
    /// </summary>
    [Get("/api/v1/roles")]
    public Task<ApiResponse<RoleDto[]>> RolesGetList();

    /// <summary>
    /// Creates a role
    /// </summary>
    [Post("/api/v1/roles")]
    public Task<ApiResponse<object>> RolesCreate(RoleDto role);

    /// <summary>
    /// Updates a role
    /// </summary>
    [Put("/api/v1/roles")]
    public Task<ApiResponse<object>> RolesUpdate(RoleDto role);

    /// <summary>
    /// Gets a single role by name
    /// </summary>
    [Get("/api/v1/roles/{name}")]
    public Task<ApiResponse<RoleDto>> RolesGetSingle(string name);

    /// <summary>
    /// Deletes a role by name
    /// </summary>
    [Delete("/api/v1/roles/{name}")]
    public Task<ApiResponse<object>> RolesDelete(string name);

    /// <summary>
    /// Gets a list of all the permissions available
    /// </summary>
    [Get("/api/v1/permissions")]
    public Task<ApiResponse<PermissionDto[]>> PermissionsGetList();
}
