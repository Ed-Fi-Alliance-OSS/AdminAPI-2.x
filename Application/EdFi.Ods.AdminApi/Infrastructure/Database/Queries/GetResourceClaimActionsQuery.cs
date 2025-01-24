// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq.Expressions;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;
using EdFi.Ods.AdminApi.Common.Settings;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetResourceClaimActionsQuery
{
    public List<ResourceClaimAction> Execute(CommonQueryParams commonQueryParams);
}

public class GetResourceClaimActionsQuery : IGetResourceClaimActionsQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;
    private readonly Dictionary<string, Expression<Func<ResourceClaimAction, object>>> _orderByColumns;

    public GetResourceClaimActionsQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
        var isSQLServerEngine = _options.Value.DatabaseEngine?.ToLowerInvariant() == DatabaseEngineEnum.SqlServer.ToLowerInvariant();
        _orderByColumns = new Dictionary<string, Expression<Func<ResourceClaimAction, object>>>
            (StringComparer.OrdinalIgnoreCase)
        {
            { SortingColumns.DefaultIdColumn, x => x.ResourceClaimActionId },
            { nameof(ResourceClaimAction.ResourceClaimId), x => x.ResourceClaimId},
            { nameof(ResourceClaimAction.ActionId), x => x.ActionId},
            { nameof(ResourceClaimAction.ValidationRuleSetName), x => isSQLServerEngine ? EF.Functions.Collate(x.ValidationRuleSetName, DatabaseEngineEnum.SqlServerCollation) : x.ValidationRuleSetName }
        };
    }

    public List<ResourceClaimAction> Execute(CommonQueryParams commonQueryParams)
    {
        Expression<Func<ResourceClaimAction, object>> columnToOrderBy = _orderByColumns.GetColumnToOrderBy(commonQueryParams.OrderBy);

        return _securityContext.ResourceClaimActions.OrderByColumn(columnToOrderBy, commonQueryParams.IsDescending)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
