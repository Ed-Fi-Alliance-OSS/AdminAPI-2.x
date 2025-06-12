// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Infrastructure.Security;
using FluentValidation;
using FluentValidation.Results;
using OpenIddict.Abstractions;

namespace EdFi.Ods.AdminApi.Infrastructure.Security;

public class ScopeValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ScopeValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only check scopes for authenticated requests to API endpoints
        if (context.User.Identity?.IsAuthenticated == true &&
            IsApiEndpoint(context.Request.Path))
        {
            // Check if user has scope claim
            if (!context.User.HasClaim(c => c.Type == OpenIddictConstants.Claims.Scope))
            {
                var validationErrors = new List<ValidationFailure>
                {
                    new()
                    {
                        PropertyName = "Scope",
                        ErrorMessage = "Required scope is missing from the token."
                    }
                };                throw new ValidationException(validationErrors);
            }

            // Get the scopes from the token
            var scopes = context.User.FindFirst(c => c.Type == OpenIddictConstants.Claims.Scope)?.Value
                .Split(' ')
                .ToList();

            // Check if user has full access scope (allows all operations)
            if (scopes == null || !scopes.Contains(SecurityConstants.Scopes.AdminApiFullAccess.Scope, StringComparer.OrdinalIgnoreCase))
            {
                // For specific endpoints, check specific scope requirements
                var requiredScope = GetRequiredScope(context.Request.Path);
                if (!string.IsNullOrEmpty(requiredScope) &&
                    (scopes == null || !scopes.Contains(requiredScope, StringComparer.OrdinalIgnoreCase)))
                {
                    var validationErrors = new List<ValidationFailure>
                    {
                        new()
                        {
                            PropertyName = "Scope",
                            ErrorMessage = $"Required scope '{requiredScope}' is missing from the token."
                        }
                    };
                    throw new ValidationException(validationErrors);
                }
            }
        }

        await _next(context);
    }

    private static bool IsApiEndpoint(PathString path)
    {
        // Check if this is an API endpoint that requires scope validation
        var pathValue = path.Value?.ToLowerInvariant();
        return pathValue != null && (
            pathValue.StartsWith("/v") || // versioned endpoints like /v2/
            pathValue.Contains("/api/") ||
            (pathValue.Contains("/") && !pathValue.Contains("health") && !pathValue.Contains("swagger") && !pathValue.Contains("connect"))        );
    }

    private static string GetRequiredScope(PathString path)
    {
        // Map specific endpoints to their required scopes based on the path
        // For now, all API endpoints require tenant access scope
        // In the future, this could be enhanced to return different scopes based on path analysis
        _ = path; // Acknowledge the parameter for future use
        return SecurityConstants.Scopes.AdminApiTenantAccess.Scope;
    }
}
