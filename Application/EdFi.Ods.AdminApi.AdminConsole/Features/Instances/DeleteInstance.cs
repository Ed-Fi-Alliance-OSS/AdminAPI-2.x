// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Features.Instances;
public class DeleteInstance : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapDelete(endpoints, "/odsInstances/{id}", Execute)
        .WithRouteOptions(b => b.WithResponseCode(202))
        .BuildForVersions(AdminApiVersions.AdminConsole);
    }

    private static async Task Execute(int id, DeleteInstanceValidator validator, IPendingDeleteInstanceCommand changeStatusInstanceCommand)
    {
        await validator.ValidateAsync(id);
        await changeStatusInstanceCommand.Execute(id);
        await Task.FromResult(Results.AcceptedAtRoute());
    }
}

public class DeleteInstanceValidator : AbstractValidator<int>
{
    private readonly IQueriesRepository<Instance> _instanceQuery;
    public DeleteInstanceValidator(IQueriesRepository<Instance> instanceQuery)
    {
        _instanceQuery = instanceQuery;

        RuleFor(x => x)
            .GreaterThan(0)
            .WithMessage(AdminConsoleValidationConstants.OdsIntanceIdIsNotValid);

        RuleFor(x => x)
            .MustAsync(IsStatusCompleted)
            .WithMessage(AdminConsoleValidationConstants.OdsIntanceIdStatusIsNotCompleted)
            .WhenAsync(Exist);

    }

    private async Task<bool> Exist(int id, CancellationToken cancellationToken)
    {
        var existingInstance = await _instanceQuery.Query()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        return existingInstance != null;
    }

    private async Task<bool> IsStatusCompleted(int id, CancellationToken cancellationToken)
    {
        var existingInstance = await _instanceQuery.Query()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        return existingInstance!.Status == InstanceStatus.Completed;
    }
}

