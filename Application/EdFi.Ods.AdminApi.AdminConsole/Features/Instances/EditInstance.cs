// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using FluentValidation;
using static EdFi.Ods.AdminApi.AdminConsole.Features.Instances.AddInstance;
using System.Dynamic;
using System.Text.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;

public class EditInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPut(endpoints, "/instances/{instanceid}", Execute)
            .WithRouteOptions(b => b.WithResponseCode(200))
            .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    public async Task<IResult> Execute(Validator validator, IEditInstanceCommand editInstanceCommand, EditInstanceRequest request, int instanceid)
    {
        await validator.GuardAsync(request);
        var instance = await editInstanceCommand.Execute(instanceid, request);
        return Results.Ok(instance);
    }

    public class EditInstanceRequest : IEditInstanceModel
    {
        [Required]
        public int DocId { get; set; }
        public int? EdOrgId { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public string Document { get; set; }
    }

    public class Validator : AbstractValidator<EditInstanceRequest>
    {
        public Validator()
        {
            RuleFor(m => m.DocId)
             .NotNull();

            RuleFor(m => m.EdOrgId)
             .NotNull();

            RuleFor(m => m.Document)
             .NotNull()
             .NotEmpty()
             .Must(BeValidDocument).WithMessage("Document must be a valid JSON.");
        }

        private bool BeValidDocument(string document)
        {
            try
            {
                JsonDocument.Parse(document);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return false;
            }
        }
    }
}
