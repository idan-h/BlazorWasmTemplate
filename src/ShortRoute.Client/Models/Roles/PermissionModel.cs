using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models.Roles;

public class PermissionModel
{
    public PermissionDto Dto { get; }
    public bool Enabled { get; set; }

    public PermissionModel(PermissionDto dto, bool enabled)
    {
        Dto = dto;
        Enabled = enabled;
    }
}
