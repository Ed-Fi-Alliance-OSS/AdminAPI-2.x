// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.InstanceManagement;
using Quartz;

namespace EdFi.Ods.AdminApi.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class InstanceManagementJob(IInstanceManagementService instanceManagementService, ILogger<InstanceManagementJob> logger) : IJob
{
    private readonly IInstanceManagementService _instanceManagementService = instanceManagementService;
    private readonly ILogger<InstanceManagementJob> _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Running scheduled instance management...");
        await _instanceManagementService.RunAsync(context.CancellationToken);
    }
}
