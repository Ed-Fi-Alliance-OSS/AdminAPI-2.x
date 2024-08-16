// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq.Expressions;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetProfilesQuery
{
    List<Profile> Execute();
    List<Profile> Execute(CommonQueryParams commonQueryParams, int? id, string? name);
}

public class GetProfilesQuery : IGetProfilesQuery
{
    private readonly IUsersContext _usersContext;
    private readonly IOptions<AppSettings> _options;
    private static readonly Dictionary<string, Expression<Func<Profile, object>>> _orderByColumnProfiles =
    new Dictionary<string, Expression<Func<Profile, object>>>
        (StringComparer.OrdinalIgnoreCase)
    {
        { "name", x => x.ProfileName },
        { "id", x => x.ProfileId }
    };
    public GetProfilesQuery(IUsersContext usersContext, IOptions<AppSettings> options)
    {
        _usersContext = usersContext;
        _options = options;
    }

    public List<Profile> Execute()
    {
        return _usersContext.Profiles.OrderBy(p => p.ProfileName).ToList();
    }
    public List<Profile> Execute(CommonQueryParams commonQueryParams, int? id, string? name)
    {
        Expression<Func<Profile, object>> columnToOrderBy = _orderByColumnProfiles.GetColumnToOrderBy(commonQueryParams.OrderBy);

        return _usersContext.Profiles
            .Where(p => id == null || p.ProfileId == id)
            .Where(p => name == null || p.ProfileName == name)
            .OrderByColumn(columnToOrderBy, commonQueryParams.IsDescending)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
