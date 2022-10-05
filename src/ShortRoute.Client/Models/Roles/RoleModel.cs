using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models.Roles;

public class RoleModel
{
    public RoleDto Dto { get; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            _onEnabledChanged?.Invoke();
        }
    }
    private bool _enabled;
    private Action? _onEnabledChanged;

    public RoleModel(RoleDto dto, bool enabled, Action? onEnabledChanged = null)
    {
        Dto = dto;
        _enabled = enabled;
        _onEnabledChanged = onEnabledChanged;
    }
}
