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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Queries;

public interface IGetOdsInstanceDerivativesQuery
{
    List<OdsInstanceDerivative> Execute();
    List<OdsInstanceDerivative> Execute(CommonQueryParams commonQueryParams);
}

public class GetOdsInstanceDerivativesQuery : IGetOdsInstanceDerivativesQuery
{
    private readonly IUsersContext _usersContext;
    private readonly IOptions<AppSettings> _options;
    private static readonly Dictionary<string, Expression<Func<OdsInstanceDerivative, object>>> _orderByColumnOds =
    new Dictionary<string, Expression<Func<OdsInstanceDerivative, object>>>
        (StringComparer.OrdinalIgnoreCase)
    {
        { "derivativeType", x => x.DerivativeType },
        { "odsInstanceId", x => x.OdsInstance.OdsInstanceId },
        { "id", x => x.OdsInstanceDerivativeId }
    };

    public GetOdsInstanceDerivativesQuery(IUsersContext usersContext, IOptions<AppSettings> options)
    {
        _usersContext = usersContext;
        _options = options;
    }

    public List<OdsInstanceDerivative> Execute()
    {
        return _usersContext.OdsInstanceDerivatives
            .Include(oid => oid.OdsInstance)
            .OrderBy(p => p.DerivativeType).ToList();
    }

    public List<OdsInstanceDerivative> Execute(CommonQueryParams commonQueryParams)
    {
        Expression<Func<OdsInstanceDerivative, object>> columnToOrderBy = _orderByColumnOds.GetColumnToOrderBy(commonQueryParams.OrderBy);

        return _usersContext.OdsInstanceDerivatives
            .Include(oid => oid.OdsInstance)
            .OrderByColumn(columnToOrderBy, commonQueryParams.IsDescending)
            .Paginate(commonQueryParams.Offset, commonQueryParams.Limit, _options)
            .ToList();
    }
}

