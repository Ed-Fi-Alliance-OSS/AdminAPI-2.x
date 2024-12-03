// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Linq;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.Common.Constants;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;
using EdFi.Ods.AdminApi.Common.Settings;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TenantEntity = EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models.Tenant;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants;

public interface IAdminConsoleTenantsService
{
    Task InitializeTenantsAsync();
    Task<List<TenantEntity>> GetTenantsAsync(bool fromCache);
    Task<TenantEntity?> GetTenantByTenantIdAsync(string tenantId);
}

public class TenantService : IAdminConsoleTenantsService
{
    private const string ADMIN_DB_KEY = "EdFi_Admin";
    private readonly IOptions<AppSettingsFile> _options;
    protected AppSettingsFile _appSettings;
    private readonly IQueriesRepository<TenantEntity> _tenantsQueryRepository;
    private readonly ICommandRepository<TenantEntity> _tenantCommandRepository;
    private readonly IMemoryCache _memoryCache;
    private static readonly ILog _log = LogManager.GetLogger(typeof(TenantService));

    public TenantService(IOptionsSnapshot<AppSettingsFile> options,
        IQueriesRepository<TenantEntity> tenantsQueryRepository,
        ICommandRepository<TenantEntity> tenantRepository,
        IMemoryCache memoryCache)
    {
        _options = options;
        _tenantsQueryRepository = tenantsQueryRepository;
        _tenantCommandRepository = tenantRepository;
        _memoryCache = memoryCache;
        _appSettings = _options.Value;
    }

    public async Task InitializeTenantsAsync()
    {
        var tenants = await GetTenantsAsync();
        var resultTenants = new List<TenantEntity>();
        dynamic adminConsoleTenant = new ExpandoObject();
        dynamic onBoarding = new ExpandoObject();
        onBoarding.status = "InProgress";
        if (tenants.Count() == 0)
        {
            //create new data
            if (_appSettings.AppSettings.MultiTenancy)
            {
                //multitenancy
                foreach (var tenantConfig in _appSettings.Tenants)
                {
                    adminConsoleTenant = new ExpandoObject();
                    adminConsoleTenant.edfiApiDiscoveryUrl = tenantConfig.Value.EdFiApiDiscoveryUrl;
                    adminConsoleTenant.tenantId = tenantConfig.Key;
                    adminConsoleTenant.onBoarding = onBoarding;
                    var dbconnection = tenantConfig.Value.ConnectionStrings.First(p => p.Key == ADMIN_DB_KEY).Value;
                    if (ConnectionStringHelper.ValidateConnectionString(_appSettings.AppSettings.DatabaseEngine!, dbconnection))
                    {
                        _tenantCommandRepository.SwitchConnectionString(dbconnection);
                        var tenant = await _tenantCommandRepository.AddAsync(
                            new TenantEntity()
                            {
                                Document = JsonConvert.SerializeObject(adminConsoleTenant)
                            });
                        await _tenantCommandRepository.SaveChangesAsync();
                        resultTenants.Add(tenant);
                    }
                }
                _tenantCommandRepository.ResetConnectionString();
            }
            else
            {
                //single data
                adminConsoleTenant.edfiApiDiscoveryUrl = _appSettings.EdFiApiDiscoveryUrl;
                adminConsoleTenant.tenantId = "default";
                adminConsoleTenant.onBoarding = onBoarding;
                var tenant = await _tenantCommandRepository.AddAsync(
                    new TenantEntity()
                    {
                        Document = JsonConvert.SerializeObject(adminConsoleTenant)
                    });
                await _tenantCommandRepository.SaveChangesAsync();

                resultTenants.Add(tenant);
            }
        }
        else
        {
            //check if all are stored
            if (_appSettings.AppSettings.MultiTenancy)
            {
                var tenantsIds = tenants.Select(p =>
                {
                    dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(p.Document)!;
                    return (string)data!.tenantId;
                });

                //Create tenants not included
                var tenantsNotIncluded = _appSettings.Tenants.Where(p => !tenantsIds.Contains(p.Key));
                foreach (var tenantConfig in tenantsNotIncluded)
                {
                    adminConsoleTenant = new ExpandoObject();
                    adminConsoleTenant.edfiApiDiscoveryUrl = tenantConfig.Value.EdFiApiDiscoveryUrl;
                    adminConsoleTenant.tenantId = tenantConfig.Key;
                    adminConsoleTenant.onBoarding = onBoarding;
                    var dbconnection = tenantConfig.Value.ConnectionStrings.First(p => p.Key == ADMIN_DB_KEY).Value;
                    if (ConnectionStringHelper.ValidateConnectionString(_appSettings.AppSettings.DatabaseEngine!, dbconnection))
                    {
                        _tenantCommandRepository.SwitchConnectionString(dbconnection);
                        var tenant = await _tenantCommandRepository.AddAsync(
                            new TenantEntity()
                            {
                                Document = JsonConvert.SerializeObject(adminConsoleTenant)
                            });
                        await _tenantCommandRepository.SaveChangesAsync();
                        resultTenants.Add(tenant);
                    }
                }

                //Update edfiurl
                var tenantsIncluded = _appSettings.Tenants.Where(p => tenantsIds.Contains(p.Key));

                foreach (var tenantConfig in tenantsIncluded)
                {
                    var tenantToUpdate = tenants.FirstOrDefault(p =>
                    {
                        dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(p.Document)!;
                        return (string)data!.tenantId == tenantConfig.Key;
                    });

                    if (tenantToUpdate != null)
                    {
                        adminConsoleTenant = JsonConvert.DeserializeObject<ExpandoObject>(tenantToUpdate.Document)!;
                        if (adminConsoleTenant!.edfiApiDiscoveryUrl != tenantConfig.Value.EdFiApiDiscoveryUrl)
                        {
                            var dbconnection = tenantConfig.Value.ConnectionStrings.First(p => p.Key == ADMIN_DB_KEY).Value;
                            if (ConnectionStringHelper.ValidateConnectionString(_appSettings.AppSettings.DatabaseEngine!, dbconnection))
                            {
                                adminConsoleTenant.edfiApiDiscoveryUrl = tenantConfig.Value.EdFiApiDiscoveryUrl;
                                tenantToUpdate.Document = JsonConvert.SerializeObject(adminConsoleTenant);
                                await _tenantsQueryRepository.SaveChangesAsync();
                            }
                        }
                    }
                }

                _tenantCommandRepository.ResetConnectionString();
            }
            resultTenants.AddRange(tenants);
        }
        //store it in memorycache
        await Task.FromResult(_memoryCache.Set(AdminConsoleConstants.TENANTS_CACHE_KEY, resultTenants));
    }

    public async Task<List<TenantEntity>> GetTenantsAsync(bool fromCache = false)
    {
        List<TenantEntity> results = new List<TenantEntity>();

        if (fromCache)
        {
            results = await GetTenantsFromCacheAsync();
            if (results.Count > 0)
            {
                return results;
            }
        }

        results = new List<TenantEntity>();
        //check multitenancy
        if (_appSettings.AppSettings.MultiTenancy)
        {
            foreach (var tenantConfig in _appSettings.Tenants.Values)
            {
                var connectionString = tenantConfig.ConnectionStrings.First(p => p.Key == ADMIN_DB_KEY).Value;
                if (ConnectionStringHelper.ValidateConnectionString(_appSettings.AppSettings.DatabaseEngine!, connectionString))
                {
                    _tenantsQueryRepository.SwitchConnectionString(connectionString);
                    var result = await _tenantsQueryRepository.GetAllAsync();
                    results.AddRange(result);
                }
            }
            _tenantsQueryRepository.ResetConnectionString();
        }
        else
        {
            var result = await _tenantsQueryRepository.GetAllAsync();
            results.AddRange(result);
        }
        return results;
    }

    public async Task<TenantEntity?> GetTenantByTenantIdAsync(string tenantId)
    {
        var tenants = await GetTenantsAsync();
        var tenant = tenants.FirstOrDefault(p =>
        {
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(p.Document)!;
            return (string)data!.tenantId == tenantId;
        });
        return tenant;
    }

    private async Task<List<Tenant>> GetTenantsFromCacheAsync()
    {
        var tenants = await Task.FromResult(_memoryCache.Get<List<Tenant>>(AdminConsoleConstants.TENANTS_CACHE_KEY));
        return tenants ?? new List<TenantEntity>();
    }
}

