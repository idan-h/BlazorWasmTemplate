using Microsoft.AspNetCore.Components;

namespace ShortRoute.Client.Components.State;

public abstract class StatefulComponent : ComponentBase//, IDisposable
{
    //protected override void OnInitialized()
    //{
    //    ProfileService.OnChange += StateHasChanged;
    //}

    //public void Dispose()
    //{
    //    ProfileService.OnChange -= StateHasChanged;
    //}
}
