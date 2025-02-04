// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq.Expressions;
using EdFi.Ods.AdminApi.Features.ResourceClaimActions;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Ods.AdminApi.Infrastructure.Helpers;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetResourceClaimActionsQuery
{
    public IReadOnlyList<ResourceClaimActionModel> Execute(CommonQueryParams commonQueryParams);
}

public class GetResourceClaimActionsQuery : IGetResourceClaimActionsQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;
    private readonly Dictionary<string, Expression<Func<ResourceClaimActionModel, object>>> _orderByColumns;

    public GetResourceClaimActionsQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
        var isSQLServerEngine = _options.Value.DatabaseEngine?.ToLowerInvariant() == DatabaseEngineEnum.SqlServer.ToLowerInvariant();
        _orderByColumns = new Dictionary<string, Expression<Func<ResourceClaimActionModel, object>>>
            (StringComparer.OrdinalIgnoreCase)
        {
            { nameof(ResourceClaimActionModel.ResourceClaimId), x => x.ResourceClaimId},
            { nameof(ResourceClaimActionModel.ResourceName), x => isSQLServerEngine ? EF.Functions.Collate(x.ResourceName, DatabaseEngineEnum.SqlServerCollation) : x.ResourceName},
        };
    }

    public IReadOnlyList<ResourceClaimActionModel> Execute(CommonQueryParams commonQueryParams)
    {
        Expression<Func<ResourceClaimActionModel, object>> columnToOrderBy = _orderByColumns.GetColumnToOrderBy(commonQueryParams.OrderBy);

        return _securityContext.ResourceClaimActions
        .Include(i => i.ResourceClaim)
        .Include(i => i.Action)
        .GroupBy(r => new { r.ResourceClaim.ResourceClaimId, r.ResourceClaim.ResourceName })
        .Select(group => new ResourceClaimActionModel
        {
            ResourceClaimId = group.Key.ResourceClaimId,
            ResourceName = group.Key.ResourceName,
            Actions = group.Select(g => new ActionForResourceClaimModel { Name = g.Action.ActionName }).Distinct().ToList()
        })
        .OrderByColumn(columnToOrderBy, commonQueryParams.IsDescending)
        .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
        .ToList();
    }
}
