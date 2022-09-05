using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using MudBlazor;
using Refit;
using ShortRoute.Contracts.Responses.Common;
using Newtonsoft.Json;

namespace ShortRoute.Client.Shared;

public static class ApiHelper
{
    public static async Task<T?> ExecuteClientCall<T>(
        this Func<Task<T>> call,
        ISnackbar snackbar,
        string? successMessage = null
        )
    {
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
                    errorObj = JsonConvert.DeserializeObject<Error>(exContent);
                }
                catch (Exception eee)
                {

                }

                if (errorObj is not null)
                {
                    foreach (var error in errorObj.Errors)
                    {
                        snackbar.Add(error, Severity.Error);
                    }

                    return default;
                }
            }

            snackbar.Add(ex.Message, Severity.Error);
            return default;
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
            return default;
        }
    }

    public static async Task<bool> ExecuteClientCall(
        this Func<Task> call,
        ISnackbar snackbar,
        string? successMessage = null
        )
    {
        try
        {
            await call();
            snackbar.Add(successMessage, Severity.Info);
            return true;
        }
        catch (ApiException ex)
        {
            if (ex.Content is string exContent)
            {
                Error? errorObj = null;
                try
                {
                    errorObj = JsonConvert.DeserializeObject<Error>(exContent);
                }
                catch (Exception eee)
                {

                }

                if (errorObj is not null)
                {
                    foreach (var error in errorObj.Errors)
                    {
                        snackbar.Add(error, Severity.Error);
                    }

                    return default;
                }
            }

            snackbar.Add(ex.Message, Severity.Error);
            return false;
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
            return false;
        }
    }
}