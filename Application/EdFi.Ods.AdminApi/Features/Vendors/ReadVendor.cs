// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Drawing;
using System.Globalization;
using AutoMapper;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Ods.AdminApi.Infrastructure.ErrorHandling;
using static EdFi.Ods.AdminApi.Features.SortingDirection;

namespace EdFi.Ods.AdminApi.Features.Vendors;

public class ReadVendor : IFeature
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        AdminApiEndpointBuilder.MapGet(endpoints, "/vendors", GetVendors)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<VendorModel[]>(200))
            .BuildForVersions(AdminApiVersions.V2);

        AdminApiEndpointBuilder.MapGet(endpoints, "/vendors/{id}", GetVendor)
            .WithDefaultDescription()
            .WithRouteOptions(b => b.WithResponse<VendorModel>(200))
            .BuildForVersions(AdminApiVersions.V2);
    }

    internal Task<IResult> GetVendors(
        IGetVendorsQuery getVendorsQuery, IMapper mapper, int offset, int limit, string? orderBy, string? direction, int? id, string? company, string? namespacePrefixes, string? contactName, string? contactEmailAddress)
    {
        var vendorList = mapper.Map<SortableList<VendorModel>>(getVendorsQuery.Execute(offset, limit, orderBy, SortingDirection.GetNonEmptyOrDefault(direction), id, company, namespacePrefixes, contactName, contactEmailAddress));
        return Task.FromResult(Results.Ok(vendorList.Sort(orderBy ?? string.Empty, SortingDirection.GetNonEmptyOrDefault(direction))));
    }

    internal Task<IResult> GetVendor(IGetVendorByIdQuery getVendorByIdQuery, IMapper mapper, int id)
    {
        var vendor = getVendorByIdQuery.Execute(id);
        if (vendor == null)
        {
            throw new NotFoundException<int>("vendor", id);
        }
        var model = mapper.Map<VendorModel>(vendor);
        return Task.FromResult(Results.Ok(model));
    }
}
