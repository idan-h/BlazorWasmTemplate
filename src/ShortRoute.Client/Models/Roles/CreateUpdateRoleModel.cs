using FluentValidation;
using Microsoft.Extensions.Localization;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Client.Shared;

namespace ShortRoute.Client.Models.Users;

public class CreateUpdateRoleModel
{
    #region Shared

    public string? RoleName { get; set; }
    public string? Description { get; set; }
    public int RoleType { get; set; }
    public List<string> PermissionNames { get; set; } = new();

    #endregion

    #region Create

    #endregion

    #region Update

    #endregion
}
