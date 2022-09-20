using Refit;
using ShortRoute.Client.Models;
using ShortRoute.Client.Models.Generic;

namespace ShortRoute.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Client Paging, Sorting and Filtering.
/// </summary>
public class EntityClientTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
    : EntityTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
{
    /// <summary>
    /// A function that loads all the data for the table from the api and returns a ListResult of TEntity.
    /// </summary>
    public Func<Task<List<TEntity>?>> LoadDataFunc { get; }

    /// <summary>
    /// A function that returns a boolean which indicates whether the supplied entity meets the search criteria
    /// (the supplied string is the search string entered).
    /// </summary>
    public Func<string?, TEntity, bool> SearchFunc { get; }

    public EntityClientTableContext(
        List<EntityField<TEntity>> fields,
        Func<Task<List<TEntity>?>> loadDataFunc,
        Func<string?, TEntity, bool> searchFunc,
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
        LoadDataFunc = loadDataFunc;
        SearchFunc = searchFunc;
    }
}