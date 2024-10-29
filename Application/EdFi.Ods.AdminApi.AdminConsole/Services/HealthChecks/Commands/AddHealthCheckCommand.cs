// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Repository;

namespace EdFi.Ods.AdminApi.AdminConsole.Services.HealthChecks.Commands;
public interface IAddHealthCheckCommand
{
    Task<HealthCheck> Execute(IAddHealthCheckModel newHealthCheck);
}

public class AddHealthCheckCommand : IAddHealthCheckCommand
{
    private readonly ICommandRepository<HealthCheck> _healtCheckCommand;
    public AddHealthCheckCommand(ICommandRepository<HealthCheck> healtCheckCommand)
    {
        _healtCheckCommand = healtCheckCommand;
    }

    public async Task<HealthCheck> Execute(IAddHealthCheckModel newHealthCheck)
    {
        return await _healtCheckCommand.AddAsync(new HealthCheck
        {
            DocId = newHealthCheck.DocId,
            InstanceId = newHealthCheck.InstanceId,
            EdOrgId = newHealthCheck.EdOrgId,
            Document = newHealthCheck.Document
        });
    }
}
public interface IAddHealthCheckModel
{
    int DocId { get; }
    int InstanceId { get; }
    int EdOrgId { get; }
    string Document { get; }
}
