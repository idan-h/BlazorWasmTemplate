using ShortRoute.Client.Infrastructure.ApiClient;

namespace ShortRoute.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Server Paging, Sorting and Filtering.
/// </summary>
public class EntityServerTableContext<TEntity, TId, TRequest, TPagination>
    : EntityTableContext<TEntity, TId, TRequest, TPagination>
{
    /// <summary>
    /// A function that loads the specified page from the api with the specified search criteria
    /// and returns a PaginatedResult of TEntity.
    /// </summary>
    public Func<TPagination, Task<PaginationResponse<TEntity>>> SearchFunc { get; }

    /// <summary>
    /// A function that exports the specified data from the API.
    /// </summary>
    //public Func<BaseFilter, Task<FileResponse>>? ExportFunc { get; }

    public bool EnableAdvancedSearch { get; }

    public EntityServerTableContext(
        List<EntityField<TEntity>> fields,
        Func<TPagination, Task<PaginationResponse<TEntity>>> searchFunc,
        //Func<BaseFilter, Task<FileResponse>>? exportFunc = null,
        bool enableAdvancedSearch = false,
        Func<TEntity, TId>? idFunc = null,
        Func<Task<TRequest>>? getDefaultsFunc = null,
        Func<TRequest, Task>? createFunc = null,
        Func<TId, Task<TRequest>>? getDetailsFunc = null,
        Func<TId, TRequest, Task>? updateFunc = null,
        Func<TId, Task>? deleteFunc = null,
        string? entityName = null,
        string? entityNamePlural = null,
        string searchPermission = "",
        string createPermission = "",
        string updatePermission = "",
        string deletePermission = "",
        string exportPermission = "",
        Func<Task>? editFormInitializedFunc = null,
        Func<bool>? hasExtraActionsFunc = null,
        Func<TEntity, bool>? canUpdateEntityFunc = null,
        Func<TEntity, bool>? canDeleteEntityFunc = null)
        : base(
            fields,
            idFunc,
            getDefaultsFunc,
            createFunc,
            getDetailsFunc,
            updateFunc,
            deleteFunc,
            entityName,
            entityNamePlural,
            searchPermission,
            createPermission,
            updatePermission,
            deletePermission,
            exportPermission,
            editFormInitializedFunc,
            hasExtraActionsFunc,
            canUpdateEntityFunc,
            canDeleteEntityFunc)
    {
        SearchFunc = searchFunc;
        //ExportFunc = exportFunc;
        EnableAdvancedSearch = enableAdvancedSearch;
    }
}