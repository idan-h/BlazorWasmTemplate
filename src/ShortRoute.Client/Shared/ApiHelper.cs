using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using MudBlazor;
using Refit;
using ShortRoute.Contracts.Responses.Common;

namespace ShortRoute.Client.Shared;

public static class ApiHelper
{
    public static async Task<T?> ExecuteClientCall<T>(
        this Func<Task<T>> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null
        )
    {
        customValidation?.ClearErrors();

        try
        {
            var result = await call();
            snackbar.Add(successMessage, Severity.Info);
            return result;
        }
        catch (ApiException ex)
        {
            if (ex.Content is string exContent)
            {
                Error? errorObj = null;
                try
                {
                    errorObj = JsonSerializer.Deserialize<Error>(ex.Content);
                }
                catch { }

                if (errorObj is not null)
                {
                    if (customValidation is not null)
                    {
                        //customValidation?.DisplayErrors(errorObj.Errors);
                    }
                    else
                    {
                        foreach (var error in errorObj.Errors)
                        {
                            snackbar.Add(error, Severity.Error);
                        }
                    }

                    return default;
                }
            }

            snackbar.Add(ex.Message, Severity.Error);
            return default;
        }
    }

    public static async Task<T?> ExecuteClientCall<T>(
        this Func<Task<ApiResponse<T>>> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null
        )
    {
        customValidation?.ClearErrors();

        var result = await call();

        if (!result.IsSuccessStatusCode)
        {
            var ex = result.Error;

            if (ex.Content is string exContent)
            {
                Error? errorObj = null;
                try
                {
                    errorObj = JsonSerializer.Deserialize<Error>(ex.Content);
                }
                catch { }

                if (errorObj is not null)
                {
                    if (customValidation is not null)
                    {
                        //customValidation?.DisplayErrors(errorObj.Errors);
                    }
                    else
                    {
                        foreach (var error in errorObj.Errors)
                        {
                            snackbar.Add(error, Severity.Error);
                        }
                    }

                    return default;
                }
            }

            snackbar.Add(result.Error.Message, Severity.Error);
            return default;
        }

        if (!string.IsNullOrWhiteSpace(successMessage))
        {
            snackbar.Add(successMessage, Severity.Info);
        }

        return result.Content;
    }

    public static async Task<bool> ExecuteClientCall(
        this Func<Task<ApiResponse<object>>> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null
        )
    {
        customValidation?.ClearErrors();

        var result = await call();

        if (!result.IsSuccessStatusCode)
        {
            var ex = result.Error;

            if (ex.Content is string exContent)
            {
                Error? errorObj = null;
                try
                {
                    errorObj = JsonSerializer.Deserialize<Error>(ex.Content);
                }
                catch { }

                if (errorObj is not null)
                {
                    if (customValidation is not null)
                    {
                        //customValidation?.DisplayErrors(errorObj.Errors);
                    }
                    else
                    {
                        foreach (var error in errorObj.Errors)
                        {
                            snackbar.Add(error, Severity.Error);
                        }
                    }

                    return false;
                }
            }

            snackbar.Add(result.Error.Message, Severity.Error);
            return false;
        }

        if (!string.IsNullOrWhiteSpace(successMessage))
        {
            snackbar.Add(successMessage, Severity.Info);
        }

        return true;
    }
}