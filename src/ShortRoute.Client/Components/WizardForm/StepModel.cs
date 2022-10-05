namespace ShortRoute.Client.Components.WizardForm;

public class StepModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public IEnumerable<string> Properties => _properties;

    private List<string> _properties = new();
    public void AddProperty(string name)
    {
        _properties.Add(name);
    }
}
