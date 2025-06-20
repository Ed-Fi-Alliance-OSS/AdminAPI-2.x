// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Common.Extensions;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public class GetApplicationByIdsQuery
{
    private readonly IUsersContext _context;

    public GetApplicationByIdsQuery(IUsersContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<Application> Execute(string applicationIds)
    {
        List<int> ids = applicationIds.Split(',').Select(int.Parse).ToList();

        var applications = _context.Applications
            .Include(a => a.ApplicationEducationOrganizations)
            .Include(a => a.Profiles)
            .Include(a => a.Vendor)
            .Include(a => a.ApiClients)
            .Where(app => ids.Contains(app.ApplicationId))
            .ToReadOnlyList();

        return applications;
    }
}
