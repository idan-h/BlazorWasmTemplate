namespace ShortRoute.Client.Components.WizardForm;

public interface IWizardForm
{
    Task Select(int step);
    int AddStep(StepModel step);
    int CurrentStep { get; }
}
