namespace ShortRoute.Client.Components.EntityTable;

public interface IAddEditModal<TRequest, TCreate, TUpdate>
{
    TRequest RequestModel { get; }
    bool IsCreate { get; }
    void ForceRender();
    void Close();
}