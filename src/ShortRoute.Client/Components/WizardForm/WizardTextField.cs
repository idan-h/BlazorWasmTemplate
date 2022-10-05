using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShortRoute.Client.Helpers;

namespace ShortRoute.Client.Components.WizardForm;

public class WizardTextField<T> : MudTextField<T>
{
    protected override void OnInitialized()
    {
        if (Step is null)
        {
            throw new InvalidOperationException($"{nameof(WizardTextField<T>)} cannot be used outside {nameof(WizardForm)}");
        }

        if (For is not null)
        {
            var name = ReflectionHelper.GetPropertyName(For);
            Step.AddProperty(name);
        }

        base.OnInitialized();
    }

    [CascadingParameter]
    public StepModel Step { get; set; } = default!;
}
