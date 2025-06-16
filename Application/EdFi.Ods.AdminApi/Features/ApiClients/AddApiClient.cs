// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Settings;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using static EdFi.Ods.AdminApi.Features.Applications.AddApplication;

namespace EdFi.Ods.AdminApi.Features.ApiClients;

public class AddApiClient : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/apiclients", Handle)
            .WithDefaultSummaryAndDescription()
            .WithRouteOptions(b => b.WithResponse<ApiClientResult>(201))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, IAddApiClientCommand addApiClientCommand, IMapper mapper, IUsersContext db, AddApiClientRequest request, IOptions<AppSettings> options)
    {
        await validator.GuardAsync(request);
        GuardAgainstInvalidEntityReferences(request, db);
        var addedApiClientResult = addApiClientCommand.Execute(request, options);
        var model = mapper.Map<ApiClientResult>(addedApiClientResult);
        return Results.Created($"/apiclients/{model.Id}", model);
    }

    private void GuardAgainstInvalidEntityReferences(AddApiClientRequest request, IUsersContext db)
    {
        if (null == db.Vendors.Find(request.VendorId))
            throw new ValidationException([new ValidationFailure(nameof(request.VendorId), $"Vendor with ID {request.VendorId} not found.")]);

        ValidateOdsInstanceIds(request, db);
    }

    private static void ValidateOdsInstanceIds(AddApiClientRequest request, IUsersContext db)
    {
        var allOdsInstanceIds = db.OdsInstances.Select(p => p.OdsInstanceId).ToList();

        if ((request.OdsInstanceIds != null && request.OdsInstanceIds.Any()) && allOdsInstanceIds.Count == 0)
        {
            throw new ValidationException([new ValidationFailure(nameof(request.OdsInstanceIds), $"The following OdsInstanceIds were not found in database: {string.Join(", ", request.OdsInstanceIds)}")]);
        }

        if ((request.OdsInstanceIds != null && request.OdsInstanceIds.Any()) && (!request.OdsInstanceIds.All(p => allOdsInstanceIds.Contains(p))))
        {
            var notExist = request.OdsInstanceIds.Where(p => !allOdsInstanceIds.Contains(p));
            throw new ValidationException([new ValidationFailure(nameof(request.OdsInstanceIds), $"The following OdsInstanceIds were not found in database: {string.Join(", ", notExist)}")]);
        }
    }

    [SwaggerSchema(Title = "AddApiClientRequest")]
    public class AddApiClientRequest : IAddApiClientModel
    {
        [SwaggerSchema(Description = FeatureConstants.ApiClientNameDescription, Nullable = false)]
        public string Name { get; set; } = string.Empty;

        [SwaggerSchema(Description = FeatureConstants.ApiClientIsApprovedDescription, Nullable = false)]
        public bool IsApproved { get; set; }

        [SwaggerSchema(Description = FeatureConstants.ApiClientApplicationIdDescription, Nullable = false)]
        public int ApplicationId { get; set; }

        [SwaggerSchema(Description = FeatureConstants.VendorIdDescription, Nullable = false)]
        public int VendorId { get; set; } = 0;

        [SwaggerSchema(Description = FeatureConstants.OdsInstanceIdsDescription, Nullable = false)]
        public IEnumerable<int>? OdsInstanceIds { get; set; }

    }

    public class Validator : AbstractValidator<AddApiClientRequest>
    {
        //Since this is a PoC, we are not implementing the full validation logic.
        public Validator()
        {
            RuleFor(m => m.Name)
             .NotEmpty();

            RuleFor(m => m.ApplicationId)
                .GreaterThan(0);

            RuleFor(m => m.VendorId)
                .GreaterThan(0);

            RuleFor(m => m.OdsInstanceIds)
                .NotEmpty()
                .WithMessage(FeatureConstants.OdsInstanceIdsValidationMessage);

        }
    }
}
