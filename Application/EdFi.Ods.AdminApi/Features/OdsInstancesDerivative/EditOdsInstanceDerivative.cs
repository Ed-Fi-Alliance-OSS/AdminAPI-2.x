// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FluentValidation;
using FluentValidation.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.AdminApi.Features.Profiles;

public class EditOdsInstanceDerivative : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder
            .MapPut(endpoints, "/odsInstancesDerivative/{id}", Handle)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponseCode(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> Handle(Validator validator, IEditOdsInstanceDerivativeCommand editOdsInstanceDerivativeCommand, IMapper mapper, IUsersContext db, EditOdsInstanceDerivativeRequest request, int id)
    {
        await validator.GuardAsync(request);
        request.Id = id;
        GuardAgainstInvalidEntityReferences(request, db);
        editOdsInstanceDerivativeCommand.Execute(request);
        return Results.Ok();
    }

    private void GuardAgainstInvalidEntityReferences(EditOdsInstanceDerivativeRequest request, IUsersContext db)
    {
        if (null == db.OdsInstances.Find(request.OdsInstanceId))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.OdsInstanceId), $"ODS instance with ID {request.OdsInstanceId} not found.") });
    }

    [SwaggerSchema(Title = "EditOdsInstanceDerivativeRequest")]
    public class EditOdsInstanceDerivativeRequest : IEditOdsInstanceDerivativeModel
    {
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeIdDescription, Nullable = false)]
        public int Id { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeOdsInstanceIdDescription, Nullable = false)]
        public int OdsInstanceId { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeDerivativeTypeDescription, Nullable = false)]
        public string? DerivativeType { get; set; }
        [SwaggerSchema(Description = FeatureConstants.OdsInstanceDerivativeConnectionStringDescription, Nullable = false)]
        public string? ConnectionString { get; set; }
    }

    public class Validator : AbstractValidator<EditOdsInstanceDerivativeRequest>
    {
        public Validator()
        {
            RuleFor(m => m.DerivativeType).NotEmpty();
            RuleFor(m => m.ConnectionString).NotEmpty();
            RuleFor(m => m.OdsInstanceId).Must(id => id > 0).WithMessage(FeatureConstants.OdsInstanceIdValidationMessage);
        }
    }
}
