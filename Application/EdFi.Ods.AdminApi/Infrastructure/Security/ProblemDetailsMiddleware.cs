// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using OpenIddict.Abstractions;
using System.Text.Json;

namespace EdFi.Ods.AdminApi.Infrastructure.Security;

public class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;

    public ProblemDetailsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Intercept the response to modify content type for invalid_scope errors
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Check if this is a token endpoint response with invalid_scope error
        if (context.Request.Path.StartsWithSegments("/connect/token") &&
            context.Response.StatusCode == 400)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

            // Check if the response contains invalid_scope error
            if (responseContent.Contains("\"error\": \"invalid_scope\""))
            {
                context.Response.ContentType = "application/problem+json";
            }

            // Write the response back
            responseBody.Seek(0, SeekOrigin.Begin);
        }

        await responseBody.CopyToAsync(originalBodyStream);
    }
}
