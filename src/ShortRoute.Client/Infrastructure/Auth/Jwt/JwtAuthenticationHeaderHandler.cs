﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using ShortRoute.Client.Infrastructure.Common;
using System.Net;

namespace ShortRoute.Client.Infrastructure.Auth.Jwt;

public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAccessTokenProviderAccessor _tokenProviderAccessor;
    private readonly NavigationManager _navigation;

    public JwtAuthenticationHeaderHandler(IAccessTokenProviderAccessor tokenProviderAccessor, NavigationManager navigation)
    {
        _tokenProviderAccessor = tokenProviderAccessor;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // skip token endpoints
        if (request.RequestUri?.AbsolutePath.Contains("/auth") is not true
            && request.RequestUri?.AbsolutePath.Contains("/refresh-auth") is not true
            && request.RequestUri?.AbsolutePath.Contains("/logout") is not true)
        {
            if (await _tokenProviderAccessor.TokenProvider.GetAccessTokenAsync() is string token)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _navigation.NavigateTo("/login", true);
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}