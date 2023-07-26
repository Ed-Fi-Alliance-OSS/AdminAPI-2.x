// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.ClaimSetEditor;

namespace EdFi.Ods.AdminApi.Features.ClaimSets.ResourceClaims;

public class EditResourceClaimActions : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapPost(endpoints, "/claimsets/{claimsetid}/resourceclaimActions", HandleAddResourceClaims)
       .WithDefaultDescription()
       .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapPut(endpoints, "/claimsets/{claimsetid}/resourceclaimActions/{resourceclaimid}", HandleEditResourceClaims)
       .WithDefaultDescription()
       .BuildForVersions(AdminApiVersions.V2);
    }

    public async Task<IResult> HandleAddResourceClaims(EditResourceClaimClaimSetValidator validator,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IMapper mapper,
        EditResourceClaimOnClaimSetRequest request, int claimsetid)
    {
        await ExecuteHandle(validator, editResourcesOnClaimSetCommand, updateResourcesOnClaimSetCommand, getResourcesByClaimSetIdQuery, mapper, request);
        return Results.Ok();
    }

    public async Task<IResult> HandleEditResourceClaims(EditResourceClaimClaimSetValidator validator,
        EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand,
        UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand,
        IGetClaimSetByIdQuery getClaimSetByIdQuery,
        IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery,
        IMapper mapper,
        EditResourceClaimOnClaimSetRequest request, int claimsetid, int resourceclaimid)
    {
        await ExecuteHandle(validator, editResourcesOnClaimSetCommand, updateResourcesOnClaimSetCommand, getResourcesByClaimSetIdQuery, mapper, request);
        var claimSet = getClaimSetByIdQuery.Execute(claimsetid);
        var model = mapper.Map<ClaimSetDetailsModel>(claimSet);
        model.ResourceClaims = getResourcesByClaimSetIdQuery.AllResources(claimsetid)
            .Select(r => mapper.Map<ResourceClaimModel>(r)).ToList();

        return Results.Ok(model);
    }

    private static async Task ExecuteHandle(EditResourceClaimClaimSetValidator validator, EditResourceOnClaimSetCommand editResourcesOnClaimSetCommand, UpdateResourcesOnClaimSetCommand updateResourcesOnClaimSetCommand, IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery, IMapper mapper, EditResourceClaimOnClaimSetRequest request)
    {
        await validator.GuardAsync(request);
        var editResourceOnClaimSetModel = mapper.Map<EditResourceOnClaimSetModel>(request);
        editResourceOnClaimSetModel.ResourceClaim!.Id = request.ResourceClaimId;
        var resourceClaims = getResourcesByClaimSetIdQuery.AllResources(request.ResourceClaimId);
        var resourceClaim = resourceClaims.FirstOrDefault(rc => rc.Id == request.ParentResourceClaimId.GetValueOrDefault());
        if (resourceClaim != null)
        {
            if (!resourceClaim.Children.Any(c => c.Id == editResourceOnClaimSetModel.ResourceClaim!.Id))
            {
                foreach (var rc in resourceClaims)
                {
                    if (rc.Id == editResourceOnClaimSetModel.ResourceClaim!.Id)
                    {
                        rc.Children.Add(editResourceOnClaimSetModel.ResourceClaim!);
                    }
                }
                updateResourcesOnClaimSetCommand.Execute(
                    new UpdateResourcesOnClaimSetModel
                    {
                        ClaimSetId = request.ClaimSetId,
                        ResourceClaims = mapper.Map<List<ResourceClaim>>(resourceClaims)
                    });
            }

        }

        editResourcesOnClaimSetCommand.Execute(editResourceOnClaimSetModel);
    }
}
