// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using static EdFi.Ods.AdminApi.Features.SortingDirection;

namespace EdFi.Ods.AdminApi.Features.Profiles;

public class ReadProfile : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/profiles", GetProfiles)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ProfileModel[]>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/profiles/{id}", GetProfile)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<ProfileDetailsModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetProfiles(IGetProfilesQuery getProfilesQuery, IMapper mapper, int offset, int limit, string? orderBy, string? sortDirection, int? id, string? name)
    {
        var profileList = mapper.Map<SortableList<ProfileModel>>(getProfilesQuery.Execute(offset, limit, id, name));
        return Task.FromResult(Results.Ok(profileList.Sort(orderBy ?? string.Empty, SortingDirection.GetNonEmptyOrDefault(sortDirection))));
    }

    internal Task<IResult> GetProfile(IGetProfileByIdQuery getProfileByIdQuery, IMapper mapper, int id)
    {
        var profile = getProfileByIdQuery.Execute(id);
        if (profile == null)
        {
            throw new NotFoundException<int>("profile", id);
        }
        var model = mapper.Map<ProfileDetailsModel>(profile);
        return Task.FromResult(Results.Ok(model));
    }
}
