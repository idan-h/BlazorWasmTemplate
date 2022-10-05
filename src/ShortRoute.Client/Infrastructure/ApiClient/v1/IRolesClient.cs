using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Commands.Authentication.Roles;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Responses.Authentication.Roles;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IRolesClient : IApiClient
{
    /// <summary>
    /// A list of all the roles the current user has permission to see
    /// </summary>
    [Get("/api/v1/roles")]
    public Task<GetRolesResponse> RolesGetList();

    /// <summary>
    /// Creates a role
    /// </summary>
    [Post("/api/v1/roles")]
    public Task RolesCreate(CreateRoleCommand createRole);

    /// <summary>
    /// Updates a role
    /// </summary>
    [Put("/api/v1/roles")]
    public Task RolesUpdate(UpdateRoleCommand updateRole);

    /// <summary>
    /// Gets a single role by name
    /// </summary>
    [Get("/api/v1/roles/{name}")]
    public Task<RoleDto> RolesGetSingle(string name);

    /// <summary>
    /// Deletes a role by name
    /// </summary>
    [Delete("/api/v1/roles/{name}")]
    public Task RolesDelete(string name);

    /// <summary>
    /// Gets a list of all the permissions available
    /// </summary>
    [Get("/api/v1/permissions")]
    public Task<GetPermissionsResponse> PermissionsGetList();
}
