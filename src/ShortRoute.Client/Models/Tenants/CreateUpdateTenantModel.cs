using FluentValidation;
using Microsoft.Extensions.Localization;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Client.Shared;

namespace ShortRoute.Client.Models.Users;

public class CreateUpdateTenantModel : BaseCreateUpdateModel<string>
{
    #region Shared

    public string? TenantFullName { get; set; }
    public IEnumerable<string> TenantRolesName { get; set; } = Enumerable.Empty<string>();

    #endregion

    #region Create

    #endregion

    #region Update

    public bool IsActive { get; set; }

    // Id in base class

    #endregion
}
