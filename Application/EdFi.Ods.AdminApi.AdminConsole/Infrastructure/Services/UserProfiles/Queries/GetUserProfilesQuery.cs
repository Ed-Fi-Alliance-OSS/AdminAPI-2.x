// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.UserProfiles.Queries;

public interface IGetUserProfilesQuery
{
    Task<IEnumerable<Permission>> Execute();
}

public class GetUserProfilesQuery : IGetUserProfilesQuery
{
    private readonly IQueriesRepository<Permission> _userProfilesQuery;

    public GetUserProfilesQuery(IQueriesRepository<Permission> userProfilesQuery)
    {
        _userProfilesQuery = userProfilesQuery;
    }
    public async Task<IEnumerable<Permission>> Execute()
    {
        return await _userProfilesQuery.GetAllAsync();
    }
}
