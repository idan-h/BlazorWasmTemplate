namespace ShortRoute.Client.Models.Generic;

public class BaseCreateUpdateModel<TId>
{
    public TId? Id { get; set; }
    public bool IsCreate => Id is null;
    public bool IsUpdate => !IsCreate;
}
