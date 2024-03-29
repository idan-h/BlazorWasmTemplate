﻿using MudBlazor;

namespace ShortRoute.Client.Components.EntityTable;

/// <summary>
/// Abstract base class for the initialization Context of the EntityTable Component.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the id of the entity.</typeparam>
/// <typeparam name="TCreateUpdateRequest">The request which updates or creates.</typeparam>
/// <typeparam name="TCreateRequest">The request object which creates.</typeparam>
/// <typeparam name="TUpdateRequest">The request object which updates.</typeparam>
public abstract class EntityTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
{
    /// <summary>
    /// The columns you want to display on the table.
    /// </summary>
    public List<EntityField<TEntity>> Fields { get; }

    /// <summary>
    /// A function that returns the Id of the entity. This is only needed when using the CRUD functionality.
    /// </summary>
    public Func<TEntity, TId>? IdFunc { get; }

    /// <summary>
    /// A function that executes the GetDefaults method on the api (or supplies defaults locally) and returns
    /// a Task of Result of the create request. Must be supplied to use <see cref="CreateFunc"/>.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<Task<TCreateUpdateRequest>>? GetDefaultsFunc { get; }

    /// <summary>
    /// A function that executes the Create method on the api with the supplied entity and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TCreateRequest, Task>? CreateFunc { get; }

    /// <summary>
    /// A function that executes the GetDetails method on the api with the supplied Id and returns a Task of Result of the update request.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, Task<TCreateUpdateRequest>>? GetDetailsFunc { get; }

    /// <summary>
    /// A function that executes the Update method on the api with the supplied entity and returns a Task of Result.
    /// When not supplied, the TEntity from the list is mapped to TCreateRequest using mapster.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, TUpdateRequest, Task>? UpdateFunc { get; }

    /// <summary>
    /// A function that executes the Delete method on the api with the supplied entity id and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, Task>? DeleteFunc { get; }

    /// <summary>
    /// The name of the entity. This is used in the title of the add/edit modal and delete confirmation.
    /// </summary>
    public string? EntityName { get; }

    /// <summary>
    /// The plural name of the entity. This is used in the "Search for ..." placeholder.
    /// </summary>
    public string? EntityNamePlural { get; }

    /// <summary>
    /// The FSHAction name of the search permission. This is FSHAction.Search by default.
    /// When empty, no search functionality will be available.
    /// When the string is "true", search funtionality will be enabled,
    /// otherwise it will only be enabled if the user has permission for this action on the EntityResource.
    /// </summary>
    public string SearchPermission { get; }

    /// <summary>
    /// The permission name of the create permission. This is FSHAction.Create by default.
    /// When empty, no create functionality will be available.
    /// When the string "true", create funtionality will be enabled,
    /// otherwise it will only be enabled if the user has permission for this action on the EntityResource.
    /// </summary>
    public string CreatePermission { get; }

    /// <summary>
    /// The permission name of the update permission. This is FSHAction.Update by default.
    /// When empty, no update functionality will be available.
    /// When the string is "true", update funtionality will be enabled,
    /// otherwise it will only be enabled if the user has permission for this action on the EntityResource.
    /// </summary>
    public string UpdatePermission { get; }

    /// <summary>
    /// The permission name of the delete permission. This is FSHAction.Delete by default.
    /// When empty, no delete functionality will be available.
    /// When the string is "true", delete funtionality will be enabled,
    /// otherwise it will only be enabled if the user has permission for this action on the EntityResource.
    /// </summary>
    public string DeletePermission { get; }

    /// <summary>
    /// The permission name of the export permission. This is FSHAction.Export by default.
    /// </summary>
    public string ExportPermission { get; }

    /// <summary>
    /// Use this if you want to run initialization during OnInitialized of the AddEdit form.
    /// </summary>
    public Func<Task>? EditFormInitializedFunc { get; }

    /// <summary>
    /// Use this if you want to check for permissions of content in the ExtraActions RenderFragment.
    /// The extra actions won't be available when this returns false.
    /// </summary>
    public Func<bool>? HasExtraActionsFunc { get; set; }

    /// <summary>
    /// Use this if you want to disable the update functionality for specific entities in the table.
    /// </summary>
    public Func<TEntity, bool>? CanUpdateEntityFunc { get; set; }

    /// <summary>
    /// Use this if you want to disable the delete functionality for specific entities in the table.
    /// </summary>
    public Func<TEntity, bool>? CanDeleteEntityFunc { get; set; }

    public EntityTableContext(
        List<EntityField<TEntity>> fields,
        Func<TEntity, TId>? idFunc,
        Func<Task<TCreateUpdateRequest>>? getDefaultsFunc,
        Func<TCreateRequest, Task>? createFunc,
        Func<TId, Task<TCreateUpdateRequest>>? getDetailsFunc,
        Func<TId, TUpdateRequest, Task>? updateFunc,
        Func<TId, Task>? deleteFunc,
        string? entityName,
        string? entityNamePlural,
        string searchPermission,
        string createPermission,
        string updatePermission,
        string deletePermission,
        string exportPermission,
        Func<Task>? editFormInitializedFunc,
        Func<bool>? hasExtraActionsFunc,
        Func<TEntity, bool>? canUpdateEntityFunc,
        Func<TEntity, bool>? canDeleteEntityFunc)
    {
        Fields = fields;
        EntityName = entityName;
        EntityNamePlural = entityNamePlural;
        IdFunc = idFunc;
        GetDefaultsFunc = getDefaultsFunc;
        CreateFunc = createFunc;
        GetDetailsFunc = getDetailsFunc;
        UpdateFunc = updateFunc;
        DeleteFunc = deleteFunc;
        SearchPermission = searchPermission;
        CreatePermission = createPermission;
        UpdatePermission = updatePermission;
        DeletePermission = deletePermission;
        ExportPermission = exportPermission;
        EditFormInitializedFunc = editFormInitializedFunc;
        HasExtraActionsFunc = hasExtraActionsFunc;
        CanUpdateEntityFunc = canUpdateEntityFunc;
        CanDeleteEntityFunc = canDeleteEntityFunc;
    }

    // AddEdit modal
    private IDialogReference? _addEditModalRef;

    internal void SetAddEditModalRef(IDialogReference dialog) =>
        _addEditModalRef = dialog;

    public IAddEditModal<TCreateUpdateRequest, TCreateRequest, TUpdateRequest> AddEditModal =>
        _addEditModalRef?.Dialog as IAddEditModal<TCreateUpdateRequest, TCreateRequest, TUpdateRequest>
        ?? throw new InvalidOperationException("AddEditModal is only available when the modal is shown.");

    // Shortcuts
    public EntityClientTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>? ClientContext =>
        this as EntityClientTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>;
    public EntityServerTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>? ServerContext =>
        this as EntityServerTableContext<TEntity, TId, TCreateUpdateRequest, TCreateRequest, TUpdateRequest>;
    public bool IsClientContext => ClientContext is not null;
    public bool IsServerContext => ServerContext is not null;

    // Advanced Search
    public bool AllColumnsChecked =>
        Fields.All(f => f.CheckedForSearch);
    public void AllColumnsCheckChanged(bool checkAll) =>
        Fields.ForEach(f => f.CheckedForSearch = checkAll);
    public bool AdvancedSearchEnabled =>
        ServerContext?.EnableAdvancedSearch is true;
    public List<string> SearchFields =>
        Fields.Where(f => f.CheckedForSearch).Select(f => f.SortLabel).ToList();
}