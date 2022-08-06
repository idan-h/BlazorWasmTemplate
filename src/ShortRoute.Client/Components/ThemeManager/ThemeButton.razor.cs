using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ShortRoute.Client.Components.ThemeManager;

public partial class ThemeButton
{
    [Parameter]
    public bool RightToLeft { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }
}