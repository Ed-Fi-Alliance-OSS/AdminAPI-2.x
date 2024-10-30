// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Routing;
using EdFi.Ods.AdminApi.AdminConsole.Services.HealthChecks.Commands;
using System.Reflection.Metadata;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Healthcheck;
    internal class AddHealthCheck : IFeature
    {
        public void MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            AdminApiAdminConsoleEndpointBuilder.MapPost(endpoints, "/healthcheck", Execute)
          .WithRouteOptions(b => b.WithResponseCode(201))
          .BuildForVersions();
        }

    public async Task<IResult> Execute(Validator validator, IAddHealthCheckCommand addHealthCheckCommand, AddHealthCheckRequest request)
        {
        await validator.GuardAsync(request);
            var addedHealthCheck = await addHealthCheckCommand.Execute(request);
            return Results.Created($"/healthcheck/{addedHealthCheck.DocId}", null);
        }

        [SwaggerSchema(Title = nameof(AddHealthCheckRequest))]
        public class AddHealthCheckRequest : IAddHealthCheckModel
        {
            [Required]
            public int DocId { get; set; }
            [Required]
            public int InstanceId { get; set; }
            [Required]
            public int EdOrgId { get; set; }
            [Required]
        public int TenantId { get; set; }
        [Required]
            public string Document { get; set; }
        }

    public class Validator : AbstractValidator<AddHealthCheckRequest>
    {
        public Validator()
        {
            RuleFor(m => m.InstanceId)
                 .NotNull();

            RuleFor(m => m.EdOrgId)
                 .NotNull();

            RuleFor(m => m.TenantId)
                 .NotNull();

            RuleFor(m => m.Document)
                 .NotNull()
                 .Must(BeValidDocument).WithMessage("Document must be a valid JSON.");
        }

        private bool BeValidDocument(string document)
        {
            try
            {
                Newtonsoft.Json.Linq.JToken.Parse(document);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return false;
            }
        }
    }
}
