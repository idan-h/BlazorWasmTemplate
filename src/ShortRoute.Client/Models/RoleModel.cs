using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models;

public class RoleModel
{
    public RoleDto Dto { get; }
    public bool Enabled { get; set; }

    public RoleModel(RoleDto dto, bool enabled)
    {
        Dto = dto;
        Enabled = enabled;
    }
}
