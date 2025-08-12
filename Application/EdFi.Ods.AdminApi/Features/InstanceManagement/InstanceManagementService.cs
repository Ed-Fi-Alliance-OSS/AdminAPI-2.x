// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.AdminConsole.InstanceMgrWorker.Configuration.Provisioners;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Queries;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants;
using EdFi.Ods.AdminApi.Common.Settings;

namespace EdFi.Ods.AdminApi.Features.InstanceManagement;

public interface IInstanceManagementService
{
    Task RunAsync(CancellationToken cancellationToken);
}

public class InstanceManagementService(
ILogger logger,
IAdminConsoleTenantsService adminConsoleTenantsService,
IGetInstancesQuery getInstancesQuery,
ICompleteInstanceCommand completeInstanceCommand,
IDeletedInstanceCommand deletedInstanceCommand,
IInstanceProvisioner instanceProvisioner,
AppSettings appSettings) : IInstanceManagementService
{
    private readonly ILogger _logger = logger;
    private readonly IAdminConsoleTenantsService _adminConsoleTenantsService = adminConsoleTenantsService;
    private readonly IGetInstancesQuery _getInstancesQuery = getInstancesQuery;
    private readonly ICompleteInstanceCommand _completeInstanceCommand = completeInstanceCommand;
    private readonly IDeletedInstanceCommand _deletedInstanceCommand = deletedInstanceCommand;
    private readonly IInstanceProvisioner _instanceProvisioner = instanceProvisioner;
    private readonly AppSettings _appSettings = appSettings;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await CreateInstances();
        await DeleteInstances();
    }

    private async Task CreateInstances()
    {
        _logger.LogInformation("Get tenants on Admin Api.");
        var tenants = await _adminConsoleTenantsService.GetTenantsAsync();

        if (tenants.Count == 0)
            _logger.LogInformation("No tenants returned from Admin Api.");
        else
        {
            foreach (var tenantName in tenants.Select(tenant => tenant.Document is IDictionary<string, object> document && document.TryGetValue("Name", out object? value) ? value?.ToString() : null))
            {
                if (!string.IsNullOrEmpty(tenantName))
                {
                    _instanceProvisioner.Tenant = tenantName;
                    var instances = await _getInstancesQuery.Execute(tenantName, AdminConsole.Infrastructure.DataAccess.Models.InstanceStatus.Pending.ToString()
                        );

                    if (instances == null || !instances.Any())
                    {
                        _logger.LogInformation("No instances pending to create found on Admin Api for tenant {TenantName}", tenantName);
                    }
                    else
                    {
                        foreach (var instance in instances)
                        {
                            var instanceName = instance.InstanceName;

                            if (!string.IsNullOrWhiteSpace(instanceName))
                            {
                                // Checks if the instance exists or it is a new instance
                                if (!_appSettings.OverrideExistingDatabase
                                    && await _instanceProvisioner.CheckDatabaseExists(instanceName))
                                {
                                    _logger.LogInformation("Processing instance with name: {InstanceName} already exists. Skipping processing", instanceName ?? "<No Name>");
                                    continue;
                                }
                                _logger.LogInformation("Processing instance with name: {InstanceName}", instanceName);

                                await _instanceProvisioner.AddDbInstanceAsync(instanceName, DbInstanceType.Minimal);

                                await _completeInstanceCommand.Execute(instance.Id, _instanceProvisioner.GetOdsConnectionString(instanceName));

                                _logger.LogInformation("Completed processing instance with name: {InstanceName}", instanceName);
                            }
                        }
                        _logger.LogInformation("Process completed.");
                    }
                }
            }
        }
    }

    private async Task DeleteInstances()
    {
        var tenants = await _adminConsoleTenantsService.GetTenantsAsync();
        var tenantNames = tenants.Select(tenant => tenant.Document is IDictionary<string, object> document && document.TryGetValue("Name", out object? value) ? value?.ToString() : null).ToList();
        foreach (var tenantName in tenantNames)
        {
            if (!string.IsNullOrEmpty(tenantName))
            {
                _instanceProvisioner.Tenant = tenantName;

                var instances = await _getInstancesQuery.Execute(tenantName, AdminConsole.Infrastructure.DataAccess.Models.InstanceStatus.Pending_Delete.ToString());

                if (instances == null || !instances.Any())
                {
                    _logger.LogInformation("No instances found on Admin Api with status == PENDING_DELETE. For Tenant {TenantName}", tenantName);
                    continue;
                }

                foreach (var instanceData in instances)
                {
                    try
                    {
                        _logger.LogInformation("Deleting instance: {InstanceName}", instanceData.InstanceName);
                        await _instanceProvisioner.DeleteDbInstancesAsync(instanceData.InstanceName);
                        // Call POST /adminconsole/instances/{id}/deleted to mark the Instance as DELETED
                        await _deletedInstanceCommand.Execute(instanceData.Id);
                        _logger.LogInformation("Instance {InstanceName} deleted successfully.", instanceData.InstanceName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete instance: {InstanceName}", instanceData);
                    }
                }
            }
        }
    }
}
