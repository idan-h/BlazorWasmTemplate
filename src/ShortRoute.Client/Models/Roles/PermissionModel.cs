using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models.Roles;

public class PermissionModel
{
    public PermissionDto Dto { get; }

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

    public PermissionModel(PermissionDto dto, bool enabled, Action? onEnabledChanged = null)
    {
        Dto = dto;
        _enabled = enabled;
        _onEnabledChanged = onEnabledChanged;
    }
}
