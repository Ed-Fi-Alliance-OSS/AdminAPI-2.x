// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq.Expressions;
using EdFi.Ods.AdminApi.Features.ResourceClaimActionAuthStrategies;
using EdFi.Ods.AdminApi.Helpers;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;
using EdFi.Security.DataAccess.Contexts;
using EdFi.Security.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetResourceClaimActionAuthorizationStrategiesQuery
{
    public List<ResourceClaimActionAuthorizationStrategies> Execute(CommonQueryParams commonQueryParams);
}

public class GetResourceClaimActionAuthorizationStrategiesQuery : IGetResourceClaimActionAuthorizationStrategiesQuery
{
    private readonly ISecurityContext _securityContext;
    private readonly IOptions<AppSettings> _options;
    private readonly Dictionary<string, Expression<Func<ResourceClaimActionAuthorizationStrategies, object>>> _orderByColumns;

    public GetResourceClaimActionAuthorizationStrategiesQuery(ISecurityContext securityContext, IOptions<AppSettings> options)
    {
        _securityContext = securityContext;
        _options = options;
        _orderByColumns = new Dictionary<string, Expression<Func<ResourceClaimActionAuthorizationStrategies, object>>>
            (StringComparer.OrdinalIgnoreCase)
        {
            { SortingColumns.DefaultIdColumn, x => x.ResourceClaimActionAuthorizationStrategyId },
            { nameof(ResourceClaimActionAuthStrategyModel.ResourceClaimActionId), x => x.ResourceClaimActionId },
            { nameof(ResourceClaimActionAuthStrategyModel.AuthorizationStrategyId), x => x.AuthorizationStrategyId}
        };
    }

    public List<ResourceClaimActionAuthorizationStrategies> Execute(CommonQueryParams commonQueryParams)
    {
        Expression<Func<ResourceClaimActionAuthorizationStrategies, object>> columnToOrderBy = _orderByColumns.GetColumnToOrderBy(commonQueryParams.OrderBy);

        return _securityContext.ResourceClaimActionAuthorizationStrategies.OrderByColumn(columnToOrderBy, commonQueryParams.IsDescending)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}
