using ShortRoute.Client.Components.EntityTable;
using ShortRoute.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Auth;
using ShortRoute.Contracts.Queries.Common;
using ShortRoute.Contracts.Queries.Authentication.Users;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Client.Models.Users;
using ShortRoute.Client.Mappings.Users;

namespace ShortRoute.Client.Pages.Identity.Users;

public partial class Users
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;

    protected EntityServerTableContext<UserDto, string, CreateUpdateUserModel> Context { get; set; } = default!;

    private bool _canExportUsers;
    private bool _canViewRoles;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;


    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        _canExportUsers = await AuthService.HasPermissionAsync(user, Permissions.ReadUsers);
        _canViewRoles = await AuthService.HasPermissionAsync(user, Permissions.ReadUsers);

        // add disabling users

        Context = new(
            entityName: L["User"],
            entityNamePlural: L["Users"],
            searchPermission: Permissions.ReadUsers,
            createPermission: Permissions.CreateUsers,
            updatePermission: Permissions.UpdateUsers,
            deletePermission: Permissions.DeleteUsers,
            exportPermission: string.Empty,
            fields: new()
            {
                new(user => user.FullName, L["FullName"]),
                new(user => user.Email, L["Email"]),
                new(user => user.TenantName, L["Tenant"]),
            },
            idFunc: u => u.Id!,
            searchFunc: async filter =>
            {
                var response = await UsersClient.UsersGetList(
                    filter?.Pagination?.ToString(),
                    filter?.Filter?.Text,
                    filter?.Sort?.ToString());

                return new PaginationResponse<UserDto>
                {
                    Data = response.Users.ToList(),
                    TotalCount = 0,
                };
            },
            getDefaultsFunc: null,
            //getDetailsFunc: async id => (await UsersClient.UsersGetSingle(id)).ToModel(),
            createFunc: async user =>
            {
                if (user.Password != user.ConfirmPassword)
                {
                    Snackbar.Add(L["Passwords aren't identical"], Severity.Error);
                    return;
                }

                var command = user.ToCreateCommand();
                await UsersClient.UsersCreate(command);
            },
            updateFunc: async (id, user) => await UsersClient.UsersUpdate(new UpdateUserCommand { }), // TODO
            deleteFunc: async id => await UsersClient.UsersDelete(id),

            hasExtraActionsFunc: () => true,
            enableSort: false,
            paginationType: typeof(OffsetPagination));
    }

    private void ViewProfile(in string userId) =>
        Navigation.NavigateTo($"/users/{userId}/profile");

    private void ManageRoles(in string userId) =>
        Navigation.NavigateTo($"/users/{userId}/roles");

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
        Context.AddEditModal.ForceRender();
    }
}