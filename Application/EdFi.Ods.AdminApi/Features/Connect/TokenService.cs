// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Authentication;
using System.Security.Claims;
using EdFi.Ods.AdminApi.Infrastructure.Context;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Infrastructure.MultiTenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.VisualBasic;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace EdFi.Ods.AdminApi.Features.Connect;

public interface ITokenService
{
    Task<ClaimsPrincipal> Handle(OpenIddictRequest request);
}

public class TokenService : ITokenService
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    private const string DENIED_AUTHENTICATION_MESSAGE = "Access Denied. Please review your information and try again.";

    public TokenService(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<ClaimsPrincipal> Handle(OpenIddictRequest request)
    {
        if (!request.IsClientCredentialsGrantType())
        {
            throw new NotImplementedException(DENIED_AUTHENTICATION_MESSAGE);
        }

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!) ??
            throw new NotFoundException<string?>("Access Denied", DENIED_AUTHENTICATION_MESSAGE);

        if (!await _applicationManager.ValidateClientSecretAsync(application, request.ClientSecret!))
        {
            throw new AuthenticationException(DENIED_AUTHENTICATION_MESSAGE);
        }

        var requestedScopes = request.GetScopes();
        var appScopes = (await _applicationManager.GetPermissionsAsync(application))
            .Where(p => p.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope))
            .Select(p => p[OpenIddictConstants.Permissions.Prefixes.Scope.Length..])
            .ToList();

        var missingScopes = requestedScopes.Where(s => !appScopes.Contains(s)).ToList();
        if (missingScopes.Any())
            throw new AuthenticationException(DENIED_AUTHENTICATION_MESSAGE);

        var displayName = await _applicationManager.GetDisplayNameAsync(application);

        var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId!, OpenIddictConstants.Destinations.AccessToken);
        identity.AddClaim(OpenIddictConstants.Claims.Name, displayName!, OpenIddictConstants.Destinations.AccessToken);

        var permissions = await _applicationManager.GetPermissionsAsync(application);
        string TenantIdentifier = permissions.FirstOrDefault(permission => permission.StartsWith("tnt")) ?? string.Empty;

        if (!string.IsNullOrEmpty(TenantIdentifier))
        {
            identity.AddClaim("Tenant", TenantIdentifier.Split(':').AsEnumerable().ElementAt(1));
            identity.SetDestinations(static claim => claim switch
            {
                _ => [Destinations.AccessToken]
            });
        }

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(requestedScopes);

        return principal;
    }
}
