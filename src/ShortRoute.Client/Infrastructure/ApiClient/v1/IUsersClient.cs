using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Responses.Authentication.Users;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IUsersClient : IApiClient
{
    /// <summary>
    /// Gets a list of all the users availbale filtered by current user's tenant
    /// </summary>
    [Get("/api/v1/users")]
    public Task<GetUsersResponse> UsersGetList(string? pagination = null, string? filter = null, string? sort = null);

    /// <summary>
    /// Creates a user
    /// </summary>
    [Post("/api/v1/users")]
    public Task UsersCreate(CreateUserCommand createUser);

    /// <summary>
    /// Updates a user
    /// </summary>
    [Put("/api/v1/users")]
    public Task UsersUpdate(UpdateUserCommand updateUser);

    /// <summary>
    /// Gets a single user by id
    /// </summary>
    [Get("/api/v1/users/{id}")]
    public Task<UserDto> UsersGetSingle(string id);

    /// <summary>
    /// Deletes a user
    /// </summary>
    [Delete("/api/v1/users/{id}")]
    public Task UsersDelete(string id);

    /// <summary>
    /// List of all the available roles for updating the user
    /// </summary>
    [Get("/api/v1/users/roles/{tenantId}")]
    public Task<string[]> UsersRolesGet(int? tenantId);

    /// <summary>
    /// Changes the user to disabled or active
    /// </summary>
    [Put("/api/v1/users/active")]
    public Task UsersChangeActive(ChangeUserActiveCommand changeUserActive);

    /// <summary>
    /// Sends an invitation to a user for the current user's tenant
    /// </summary>
    [Post("/api/v1/users/invite")]
    public Task InviteUser(InviteUserToTenantCommand inviteUserToTenant);

    /// <summary>
    /// Accepts the invitation of the user
    /// </summary>
    [Post("/api/v1/users/accept-invite")]
    public Task<string> AcceptInvitationPost(AcceptInvitationDto acceptInvitation);
}