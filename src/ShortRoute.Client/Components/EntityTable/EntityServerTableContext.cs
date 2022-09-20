using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Contracts.Queries.Common;

namespace ShortRoute.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Server Paging, Sorting and Filtering.
/// </summary>
public class EntityServerTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
    : EntityTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
{
    /// <summary>
    /// A function that loads the specified page from the api with the specified search criteria
    /// and returns a PaginatedResult of TEntity.
    /// </summary>
    public Func<GeneralFilter, Task<PaginationResponse<TEntity>>> SearchFunc { get; }

    /// <summary>
    /// A function that exports the specified data from the API.
    /// </summary>
    public Func<GeneralFilter, Task<HttpContent>>? ExportFunc { get; }

    public bool EnableAdvancedSearch { get; }

    public bool EnableSort { get; }

    public Type? PaginationType { get; }

    public EntityServerTableContext(
        List<EntityField<TEntity>> fields,
        Func<GeneralFilter, Task<PaginationResponse<TEntity>>> searchFunc,
        Func<GeneralFilter, Task<HttpContent>>? exportFunc = null,
        bool enableAdvancedSearch = false,
        bool enableSort = true,
        Type? paginationType = null,

        Func<TEntity, TId>? idFunc = null,
        Func<Task<TCreateUpdateRequest>>? getDefaultsFunc = null,
        Func<TCreateRequest, Task>? createFunc = null,
        Func<TId, Task<TCreateUpdateRequest>>? getDetailsFunc = null,
        Func<TId, TUpdateRequest, Task>? updateFunc = null,
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
        ExportFunc = exportFunc;
        EnableAdvancedSearch = enableAdvancedSearch;
        EnableSort = enableSort;
        PaginationType = paginationType;
    }
}