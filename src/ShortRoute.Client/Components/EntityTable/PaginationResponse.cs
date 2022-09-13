namespace ShortRoute.Client.Components.EntityTable;

public class PaginationResponse<T>
{
    public List<T> Data { get; set; } = default!;
    public int TotalCount { get; set; }
}
