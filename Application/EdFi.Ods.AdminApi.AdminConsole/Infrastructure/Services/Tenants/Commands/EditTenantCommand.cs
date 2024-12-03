// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Tenants.Commands;

public interface IEditTenantCommand
{
    Task<Tenant> ExecuteEditOnBoardingAsync(ExpandoObject model);
}

public class EditTenantCommand : IEditTenantCommand
{
    private readonly IQueriesRepository<Tenant> _queriesRepository;

    public EditTenantCommand(IQueriesRepository<Tenant> queriesRepository)
    {
        _queriesRepository = queriesRepository;
    }

    public async Task<Tenant> ExecuteEditOnBoardingAsync(ExpandoObject model)
    {
        //per tenant we should have only one
        var tenant = _queriesRepository.Query()
            .FirstOrDefault() ?? throw new NotFoundException<int>("adminconsole.tenant", 0);

        dynamic dmodel = model;

        var serializeModel = System.Text.Json.JsonSerializer.Serialize(dmodel.onBoarding);

        dynamic document = JsonConvert.DeserializeObject<ExpandoObject>(tenant.Document)!;
        document.onBoarding = JsonConvert.DeserializeObject<ExpandoObject>(serializeModel);

        tenant.Document = JsonConvert.SerializeObject(document);

        await _queriesRepository.SaveChangesAsync();

        return tenant;
    }
}

public interface IEditTenantModel
{
    string TenantId { get; }
    ExpandoObject OnBoarding { get; }
}
