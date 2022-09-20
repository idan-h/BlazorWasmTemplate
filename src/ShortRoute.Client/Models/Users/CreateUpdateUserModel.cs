using FluentValidation;
using Microsoft.Extensions.Localization;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Client.Shared;

namespace ShortRoute.Client.Models.Users;

public class CreateUpdateUserModel : BaseCreateUpdateModel<string>
{
    #region Shared

    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public IEnumerable<string> RoleNames { get; set; } = Array.Empty<string>();

    #endregion

    #region Create

    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public int? TenantId { get; set; }

    #endregion

    #region Update

    public string? TenantName { get; set; }

    public bool IsActive { get; set; }

    // Id in base class

    #endregion
}

public class CreateUpdateUserModelValidator : AbstractValidator<CreateUpdateUserModel>
{
    public CreateUpdateUserModelValidator(IStringLocalizer<SharedResource> L)
    {
        RuleFor(r => r.ConfirmPassword)
            .Equal(r => r.Password)
            .WithMessage(L["Passwords aren't equal"]);
    }
}
