using Refit;
using ShortRoute.Contracts.Commands.Authentication;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IUsersClient
{
    /// <summary>
    /// Gets a list of all the users availbale filtered by current user's tenant
    /// </summary>
    [Get("/api/v1/users")]
    public Task<ApiResponse<UserDto[]>> UsersGetList();

    /// <summary>
    /// Updates a user
    /// </summary>
    [Put("/api/v1/users")]
    public Task<ApiResponse<object>> UsersUpdate(UserDto user);

    /// <summary>
    /// Gets a single user by id
    /// </summary>
    [Get("/api/v1/users/{id}")]
    public Task<ApiResponse<UserDto>> UsersGetSingle(string id);

    /// <summary>
    /// Deletes a user
    /// </summary>
    [Delete("/api/v1/users/{id}")]
    public Task<ApiResponse<object>> UsersDelete(string id);

    /// <summary>
    /// Sends an invitation to a user for the current user's tenant
    /// </summary>
    [Post("/api/v1/users/invite")]
    public Task<ApiResponse<object>> InviteUser(InviteUserToTenantCommand inviteUserToTenant);

    /// <summary>
    /// Accepts the invitation of the user
    /// </summary>
    [Post("/api/v1/users/accept-invite")]
    public Task<ApiResponse<string>> AcceptInvitationPost(AcceptInvitationDto acceptInvitation);

    /// <summary>
    /// Accepts the invitation - returns back the code, for test purposes only
    /// </summary>
    [Get("/api/v1/users/accept-invite")]
    public Task<ApiResponse<string>> AcceptInvitationGet();

    /// <summary>
    /// Syncs the users with the login provider's users
    /// </summary>
    [Post("/api/v1/users/sync")]
    public Task<ApiResponse<object>> SyncUsers();
}