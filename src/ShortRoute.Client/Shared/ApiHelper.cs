using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using MudBlazor;
using Refit;
using ShortRoute.Contracts.Responses.Common;
using Newtonsoft.Json;
using ShortRoute.Client.Helpers;
using Microsoft.Extensions.Localization;
using ShortRoute.Contracts;
using ShortRoute.Client.Infrastructure.Auth.Exceptions;

namespace ShortRoute.Client.Shared;

public static class ApiHelper
{
    public static async Task<object?> _executeClientCall<T>(
        Func<Task<T>> valueTask,
        Func<Task> emptyTask,
        ISnackbar snackbar,
        string? successMessage = null
        )
    {
        var isValueTask = valueTask is not null;

        object? _result(T? result) => isValueTask ? result : true;

        object? _default() => isValueTask ? default : false;

        try
        {
            T? result = default;

            if (isValueTask)
            {
                result = await valueTask!();
            }
            else
            {
                await emptyTask();
            }

            snackbar.Add(successMessage, Severity.Info);
            return _result(result);
        }
        catch (ApiException ex)
        {
            var localizer = ServiceHelper.GetRequiredService<IStringLocalizer<SharedResource>>();

            if (ex.Content is string exContent)
            {
                Error? errorObj = null;
                try
                {
                    errorObj = JsonConvert.DeserializeObject<Error>(exContent);
                }
                catch { }

                if (errorObj is not null)
                {
                    foreach (var error in errorObj.Errors)
                    {
                        snackbar.Add(error, Severity.Error);
                    }

                    return _default();
                }
            }

            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                snackbar.Add(localizer[Messages.PermissionNotAllowed], Severity.Error);
                return _default();
            }

            snackbar.Add(ex.Message, Severity.Error);
        }
        catch (TenantExpiredException)
        {
            var localizer = ServiceHelper.GetRequiredService<IStringLocalizer<SharedResource>>();

            snackbar.Add(localizer[Messages.TenantExpired], Severity.Error);
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
        }

        return _default();
    }

    public static async Task<T?> ExecuteClientCall<T>(
        this Func<Task<T>> call,
        ISnackbar snackbar,
        string? successMessage = null
        )
    {
        var result = await _executeClientCall(call, null!, snackbar, successMessage);

        if (result is T t)
        {
            return t;
        }

        return default;
    }

    public static async Task<bool> ExecuteClientCall(
        this Func<Task> call,
        ISnackbar snackbar,
        string? successMessage = null
        )
    {
        return (bool)(await _executeClientCall<bool>(null!, call, snackbar, successMessage))!;
    }

    //public static async Task<T?> ExecuteClientCall<T>(
    //    this Func<Task<T>> call,
    //    ISnackbar snackbar,
    //    string? successMessage = null
    //    )
    //{
    //    try
    //    {
    //        var result = await call();
    //        snackbar.Add(successMessage, Severity.Info);
    //        return result;
    //    }
    //    catch (ApiException ex)
    //    {
    //        var localizer = ServiceHelper.GetRequiredService<IStringLocalizer<SharedResource>>();

    //        if (ex.Content is string exContent)
    //        {
    //            Error? errorObj = null;
    //            try
    //            {
    //                errorObj = JsonConvert.DeserializeObject<Error>(exContent);
    //            }
    //            catch (Exception eee)
    //            {

    //            }

    //            if (errorObj is not null)
    //            {
    //                foreach (var error in errorObj.Errors)
    //                {
    //                    snackbar.Add(error, Severity.Error);
    //                }

    //                return default;
    //            }
    //        }

    //        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
    //            || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
    //        {
    //            snackbar.Add(localizer[Messages.PermissionNotAllowed], Severity.Error);
    //            return default;
    //        }

    //        snackbar.Add(ex.Message, Severity.Error);
    //        return default;
    //    }
    //    catch (Exception ex)
    //    {
    //        snackbar.Add(ex.Message, Severity.Error);
    //        return default;
    //    }
    //}

    //public static async Task<bool> ExecuteClientCall(
    //    this Func<Task> call,
    //    ISnackbar snackbar,
    //    string? successMessage = null
    //    )
    //{
    //    try
    //    {
    //        await call();
    //        snackbar.Add(successMessage, Severity.Info);
    //        return true;
    //    }
    //    catch (ApiException ex)
    //    {
    //        if (ex.Content is string exContent)
    //        {
    //            Error? errorObj = null;
    //            try
    //            {
    //                errorObj = JsonConvert.DeserializeObject<Error>(exContent);
    //            }
    //            catch (Exception eee)
    //            {

    //            }

    //            if (errorObj is not null)
    //            {
    //                foreach (var error in errorObj.Errors)
    //                {
    //                    snackbar.Add(error, Severity.Error);
    //                }

    //                return default;
    //            }
    //        }

    //        snackbar.Add(ex.Message, Severity.Error);
    //        return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        snackbar.Add(ex.Message, Severity.Error);
    //        return false;
    //    }
    //}
}