using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShortRoute.Client.Mappings.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using ShortRoute.Contracts;

namespace ShortRoute.Client.Components.EntityTable;

public partial class AddEditModal<TRequest, TCreate, TUpdate> : IAddEditModal<TRequest, TCreate, TUpdate>
    where TCreate : new()
    where TUpdate : new()
{
    [Parameter]
    [EditorRequired]
    public RenderFragment<TRequest> ChildContent { get; set; } = default!;
    [Parameter]
    public RenderFragment<TRequest> ExtraButtons { get; set; } = default!;
    [Parameter]
    [EditorRequired]
    public TRequest RequestModel { get; set; } = default!;
    [Parameter]
    public Func<TCreate, Task> CreateFunc { get; set; } = default!;
    [Parameter]
    public Func<TUpdate, Task> UpdateFunc { get; set; } = default!;
    [Parameter]
    public Func<Task>? OnInitializedFunc { get; set; }
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = default!;
    [Parameter]
    public RenderFragment<TRequest> TitleExtraContent { get; set; } = default!;
    [Parameter]
    public bool IsCreate { get; set; }
    [Parameter]
    public string? SuccessMessage { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private EditContext EditContext { get; set; } = default!;
    private ValidationMessageStore _messageStore = default!;
    [Inject]
    IEnumerable<IValidator<TRequest>> RequestValidators { get; set; } = default!;
    [Inject]
    IEnumerable<IValidator<TCreate>> CreateValidators { get; set; } = default!;
    [Inject]
    IEnumerable<IValidator<TUpdate>> UpdateValidators { get; set; } = default!;
    private bool _alreadyValidated = false;

    public void ForceRender()
    {
        MudDialog.ForceRender();
        StateHasChanged();
    }
    public void Close() => MudDialog.Close();

    protected override Task OnInitializedAsync()
    {
        EditContext = new EditContext(RequestModel!);
        _messageStore = new(EditContext);

        EditContext.OnFieldChanged += (sender, eventArgs) =>
        {
            if (!_alreadyValidated)
            {
                return;
            }

            RequestAndValidate();
        };

        return OnInitializedFunc is not null
            ? OnInitializedFunc()
            : Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        var (create, update) = RequestAndValidate();

        if (await ApiHelper.ExecuteClientCall(
            () =>
            {
                if (create is not null)
                {
                    return CreateFunc(create);
                }

                if (update is not null)
                {
                    return UpdateFunc(update);
                }

                throw new InvalidOperationException(L["Action failed"]);
            },
            Snackbar,
            SuccessMessage))
        {
            Close();
        }
    }

    private (TCreate?, TUpdate?) RequestAndValidate()
    {
        _messageStore.Clear();

        var requestValidation = Validate(RequestModel, RequestValidators);

        TCreate? create = default;
        TUpdate? update = default;

        // For MapDynamically to work, must create mapping method from the entity to the request
        if (IsCreate)
        {
            create = RequestModel.MapDynamically<TCreate, TRequest>();
            create = Validate(create, CreateValidators) && requestValidation ? create : default;
        }
        else
        {
            update = RequestModel.MapDynamically<TUpdate, TRequest>();
            update = Validate(update, UpdateValidators) && requestValidation ? update : default;
        }

        return (create, update);
    }

    private bool Validate<TModel>(TModel model, IEnumerable<IValidator<TModel>> validators)
    {
        _alreadyValidated = true;

        if (validators?.Any() == true)
        {
            var context = new ValidationContext<TModel>(model);

            var validationResults = validators.Select(v => v.Validate(context));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            foreach (var failure in failures)
            {
                _messageStore.Add(EditContext.Field(failure.PropertyName), failure.ErrorMessage);
            }

            EditContext.NotifyValidationStateChanged();

            return !failures.Any();
        }

        return true;
    }
}