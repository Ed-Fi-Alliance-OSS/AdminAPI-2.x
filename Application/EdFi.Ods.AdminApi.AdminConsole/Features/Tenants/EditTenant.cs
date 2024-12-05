// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Documentation;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Tenants;
public class EditTenant : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPatch(endpoints, "/tenants", Execute)
            .WithRouteOptions(b => b.WithResponseCode(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    [SwaggerRequestExample(typeof(object), typeof(TenantRequestSwaggerExample))]
    public async Task<IResult> Execute(Validator validator,
        IEditTenantCommand editTenantCommand,
        IMemoryCache memoryCache,
        ExpandoObject request)
    {
        validator.Validate(request);
        await editTenantCommand.ExecuteEditOnBoardingAsync(request);
        memoryCache.Remove(AdminConsoleConstants.TenantsCacheKey);
        return Results.Ok();
    }

    public class Validator : AbstractValidator<ExpandoObject>
    {
        public Validator()
        {
            RuleFor(m => m)
                 .NotNull()
                 .Must(BeValidDocument).WithMessage("Document must be a valid JSON.");
        }

        private bool BeValidDocument(ExpandoObject document)
        {
            try
            {
                Newtonsoft.Json.Linq.JToken.Parse(JsonConvert.SerializeObject(document));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}


