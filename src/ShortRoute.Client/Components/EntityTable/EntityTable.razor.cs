using ShortRoute.Client.Components.Dialogs;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Models;
using ShortRoute.Contracts.Queries.Common;
using ShortRoute.Client.Mappings.Generic;

namespace ShortRoute.Client.Components.EntityTable;

public partial class EntityTable<TEntity, TId, TRequest, TCreate, TUpdate>
    where TRequest : new()
    where TCreate : new()
    where TUpdate : new()
{
    [Parameter]
    [EditorRequired]
    public EntityTableContext<TEntity, TId, TRequest, TCreate, TUpdate> Context { get; set; } = default!;

    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public string? SearchString { get; set; }
    [Parameter]
    public EventCallback<string> SearchStringChanged { get; set; }

    [Parameter]
    public RenderFragment? AdvancedSearchContent { get; set; }

    [Parameter]
    public RenderFragment<TEntity>? ActionsContent { get; set; }
    [Parameter]
    public RenderFragment<TEntity>? ExtraActions { get; set; }
    [Parameter]
    public RenderFragment<TEntity>? ChildRowContent { get; set; }

    [Parameter]
    public RenderFragment<TRequest>? EditFormContent { get; set; }

    [Parameter]
    public RenderFragment<TRequest>? FormExtraButtons { get; set; }

    [Parameter]
    public RenderFragment<TRequest>? FormTitleExtraContent { get; set; }


    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canSearch;
    private bool _canCreate;
    private bool _canUpdate;
    private bool _canDelete;
    private bool _canExport;

    private bool _advancedSearchExpanded;

    private MudTable<TEntity> _table = default!;
    private IEnumerable<TEntity>? _entityList;
    private int _totalItems;

    private Sort? _lastSort;
    private int? _lastPage;
    private TableIdPaginationData _lastIdData = new();
    //private Dictionary<int, string?> _lastIdByPage = new()
    //{
    //    [0] = null
    //};

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = await CanDoActionAsync(Context.SearchPermission, state);
        _canCreate = await CanDoActionAsync(Context.CreatePermission, state);
        _canUpdate = await CanDoActionAsync(Context.UpdatePermission, state);
        _canDelete = await CanDoActionAsync(Context.DeletePermission, state);
        _canExport = await CanDoActionAsync(Context.ExportPermission, state);

        await LocalLoadDataAsync();
    }

    public Task ReloadDataAsync() =>
        Context.IsClientContext
            ? LocalLoadDataAsync()
            : ServerLoadDataAsync();

    private async Task<bool> CanDoActionAsync(string action, AuthenticationState state)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return false;
        }

        return action.ToLower() == "true" || await AuthService.HasPermissionAsync(state.User, action);
    }

    private bool HasActions => _canUpdate || _canDelete || HasExtraActions;
    private bool HasExtraActions => Context.HasExtraActionsFunc?.Invoke() == true;
    private bool CanUpdateEntity(TEntity entity) => _canUpdate && (Context.CanUpdateEntityFunc is null || Context.CanUpdateEntityFunc(entity));
    private bool CanDeleteEntity(TEntity entity) => _canDelete && (Context.CanDeleteEntityFunc is null || Context.CanDeleteEntityFunc(entity));

    // Client side paging/filtering
    private bool LocalSearch(TEntity entity) =>
        Context.ClientContext?.SearchFunc is { } searchFunc
            ? searchFunc(SearchString, entity)
            : string.IsNullOrWhiteSpace(SearchString);

    private async Task LocalLoadDataAsync()
    {
        if (Loading || Context.ClientContext is null)
        {
            return;
        }

        Loading = true;

        if (await ApiHelper.ExecuteClientCall(
                () => Context.ClientContext.LoadDataFunc(), Snackbar)
            is List<TEntity> result)
        {
            _entityList = result;
        }

        Loading = false;
    }

    // Server Side paging/filtering

    private async Task OnSearchStringChanged(string? text = null)
    {
        await SearchStringChanged.InvokeAsync(SearchString);

        // Check if the user is still typing and when he stops load the data from the server
        await Task.Delay(500);
        if (SearchString != text)
        {
            return;
        }

        await ServerLoadDataAsync();
    }

    private async Task ServerLoadDataAsync()
    {
        if (Context.IsServerContext)
        {
            await _table.ReloadServerData();
        }
    }

    private Func<TableState, Task<TableData<TEntity>>>? ServerReloadFunc =>
        Context.IsServerContext ? ServerReload : null;

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!Loading && Context.ServerContext is not null)
        {
            Loading = true;

            var filter = GetFilter(state);

            if (await ApiHelper.ExecuteClientCall(
                    () => Context.ServerContext.SearchFunc(filter), Snackbar)
                is { } result)
            {
                _totalItems = result.TotalCount;
                _entityList = result.Data;
            }

            Loading = false;
        }

        return new TableData<TEntity> { TotalItems = _totalItems, Items = _entityList };
    }

    private async Task ExportAsync()
    {
        if (!Loading && Context.ServerContext is not null)
        {
            if (Context.ServerContext.ExportFunc is not null)
            {
                Loading = true;

                var filter = GetFilter();

                if (await ApiHelper.ExecuteClientCall(() => Context.ServerContext.ExportFunc(filter), Snackbar)
                    is { } result)
                {
                    try
                    {
                        using var streamRef = new DotNetStreamReference(await result.ReadAsStreamAsync());
                        await JS.InvokeVoidAsync("downloadFileFromStream", $"{Context.EntityNamePlural}.xlsx", streamRef);
                    }
                    catch
                    {
                        Snackbar.Add(L["Error has occured while downloading"], Severity.Error);
                    }
                }

                Loading = false;
            }
        }
    }

    private GeneralFilter GetFilter(TableState state)
    {
        string[]? orderings = null;
        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            orderings = new[] { state.SortLabel + (state.SortDirection == SortDirection.Descending ? "*" : "") };
        }
        _lastSort = new Sort { Fields = orderings ?? Enumerable.Empty<string>() };

        var lastIdData = GetCurrentPageLastId(orderings, state);

        var pagination = new Pagination
        {
            PageNum = state.Page + 1,
            LastId = lastIdData.Id,
            LastIdDesc = lastIdData.Desc,
            PageSize = state.PageSize
        };

        _lastPage = state.Page;

        //if (!Context.AllColumnsChecked)
        //{
        //    filter.AdvancedSearch = new()
        //    {
        //        Fields = Context.SearchFields,
        //        Keyword = filter.Keyword
        //    };
        //    filter.Keyword = null;
        //}

        return new GeneralFilter
        {
            Pagination = pagination,
            Filter = new TextFilter { Text = SearchString },
            Sort = _lastSort,
        };
    }
    private GeneralFilter GetFilter()
    {
        //if (!Context.AllColumnsChecked)
        //{
        //    filter.AdvancedSearch = new()
        //    {
        //        Fields = Context.SearchFields,
        //        Keyword = filter.Keyword
        //    };
        //    filter.Keyword = null;
        //}

        return new GeneralFilter
        {
            Filter = new TextFilter { Text = SearchString },
            Sort = _lastSort,
        };
    }
    private TableIdPaginationData GetCurrentPageLastId(string[]? orderings, TableState state)
    {
        if (orderings?.Any() == true || state.Page <= 0)
        {
            _lastPage = null;
            return new();
        }

        var isDesc = false;
        TEntity? entity = default;

        // Next page
        if (state.Page - 1 == _lastPage)
        {
            if (_entityList?.Any() == true)
            {
                entity = _entityList.Last();
            }
        }
        // Previous page
        else if (state.Page + 1 == _lastPage)
        {
            if (_entityList?.Any() == true)
            {
                entity = _entityList.First();
                isDesc = true;
            }
        }
        // Current page
        else if (state.Page == _lastPage)
        {
            return _lastIdData;
        }
        // Last page
        else
        {
            return new()
            {
                Id = "_"
            };
        }

        var idFunc = Context.ServerContext?.IdFunc;

        if (entity is null || idFunc is null)
        {
            return new();
        }

        _lastIdData = new()
        {
            Id = idFunc.Invoke(entity)?.ToString(),
            Desc = isDesc
        };

        return _lastIdData;
    }

    private async Task InvokeModal(TEntity? entity = default)
    {
        bool isCreate = entity is null;

        var parameters = new DialogParameters()
        {
            { nameof(AddEditModal<TRequest, TCreate, TUpdate>.ChildContent), EditFormContent },
            { nameof(AddEditModal<TRequest, TCreate, TUpdate>.ExtraButtons), FormExtraButtons },
            { nameof(AddEditModal<TRequest, TCreate, TUpdate>.TitleExtraContent), FormTitleExtraContent },
            { nameof(AddEditModal<TRequest, TCreate, TUpdate>.OnInitializedFunc), Context.EditFormInitializedFunc },
            { nameof(AddEditModal<TRequest, TCreate, TUpdate>.IsCreate), isCreate }
        };

        Func<TCreate, Task>? createFunc = null;
        Func<TUpdate, Task>? updateFunc = null;
        TRequest requestModel;
        string title, successMessage;

        if (isCreate)
        {
            _ = Context.CreateFunc ?? throw new InvalidOperationException("CreateFunc can't be null!");

            createFunc = Context.CreateFunc;

            requestModel =
                Context.GetDefaultsFunc is not null
                    && await ApiHelper.ExecuteClientCall(() => Context.GetDefaultsFunc(), Snackbar)
                        is { } defaultsResult
                ? defaultsResult
                : new TRequest();

            title = $"{L["Create"]} {Context.EntityName}";
            successMessage = $"{Context.EntityName} {L["Created"]}";
        }
        else
        {
            _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
            _ = Context.UpdateFunc ?? throw new InvalidOperationException("UpdateFunc can't be null!");

            var id = Context.IdFunc(entity!);

            updateFunc = update => Context.UpdateFunc(id, update);

            requestModel =
                Context.GetDetailsFunc is not null
                    && await ApiHelper.ExecuteClientCall(() => Context.GetDetailsFunc(id!), Snackbar)
                        is { } detailsResult
                ? detailsResult
                : entity!.MapDynamically<TRequest, TEntity>();
            // For MapDynamically to work, must create mapping method from the entity to the request

            title = $"{L["Edit"]} {Context.EntityName}";
            successMessage = $"{Context.EntityName} {L["Updated"]}";
        }

        parameters.Add(nameof(AddEditModal<TRequest, TCreate, TUpdate>.CreateFunc), createFunc);
        parameters.Add(nameof(AddEditModal<TRequest, TCreate, TUpdate>.UpdateFunc), updateFunc);
        parameters.Add(nameof(AddEditModal<TRequest, TCreate, TUpdate>.RequestModel), requestModel);
        parameters.Add(nameof(AddEditModal<TRequest, TCreate, TUpdate>.Title), title);
        parameters.Add(nameof(AddEditModal<TRequest, TCreate, TUpdate>.SuccessMessage), successMessage);

        var dialog = DialogService.ShowModal<AddEditModal<TRequest, TCreate, TUpdate>>(parameters);

        Context.SetAddEditModalRef(dialog);

        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await ReloadDataAsync();
        }
    }

    private async Task Delete(TEntity entity)
    {
        _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
        TId id = Context.IdFunc(entity);

        string deleteContent = L["You're sure you want to delete {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, Context.EntityName, id) }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            _ = Context.DeleteFunc ?? throw new InvalidOperationException("DeleteFunc can't be null!");

            await ApiHelper.ExecuteClientCall(() => Context.DeleteFunc(id), Snackbar);

            await ReloadDataAsync();
        }
    }

    private class TableIdPaginationData
    {
        public string? Id { get; set; }
        public bool Desc { get; set; }
    }
}