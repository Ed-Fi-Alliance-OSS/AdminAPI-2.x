// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.AdminApi.Features.ApiClients;

public class ReadApliClientn : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/apiclients", GetApiClients)
            .WithDefaultSummaryAndDescription()
            .WithRouteOptions(b => b.WithResponse<ApiClientModel[]>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/apiclients/{id}", GetApiClient)
            .WithDefaultSummaryAndDescription()
            .WithRouteOptions(b => b.WithResponse<ApiClientModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetApiClients(
        GetApiClientsByApplicationIdQuery getApiClientsByApplicationIdQuery,
        IMapper mapper,
        [FromQuery(Name = "applicationid")] int applicationid)
    {
        var applications = mapper.Map<List<ApiClientModel>>(getApiClientsByApplicationIdQuery.Execute(applicationid));
        return Task.FromResult(Results.Ok(applications));
    }

    internal Task<IResult> GetApiClient(GetApiClientByIdQuery getApiClientByIdQuery, IMapper mapper, int id)
    {
        var apiClient = getApiClientByIdQuery.Execute(id) ?? throw new NotFoundException<int>("apiClient", id);
        var model = mapper.Map<ApiClientModel>(apiClient);
        return Task.FromResult(Results.Ok(model));
    }
}
