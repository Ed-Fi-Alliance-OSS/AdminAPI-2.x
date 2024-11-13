// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Queries;

public interface IGetStepQuery
{
    Task<Step> Execute(int tenantId);
}

public class GetStepQuery : IGetStepQuery
{
    private readonly IQueriesRepository<Step> _stepQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetStepQuery(IQueriesRepository<Step> stepQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _stepQuery = stepQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Step> Execute(int tenantId)
    {

        var step = await _stepQuery.Query().SingleOrDefaultAsync(step => step.TenantId == tenantId);

        if (step == null)
            return null;

        JsonNode? jnDocument = JsonNode.Parse(step.Document);

        var encryptedClientId = jnDocument!["clientId"]?.AsValue().ToString();
        var encryptedClientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

        var clientId = string.Empty;
        var clientSecret = string.Empty;

        if (!string.IsNullOrEmpty(encryptedClientId) && !string.IsNullOrEmpty(encryptedClientSecret))
        {
            _encryptionService.TryDecrypt(encryptedClientId, _encryptionKey, out clientId);
            _encryptionService.TryDecrypt(encryptedClientSecret, _encryptionKey, out clientSecret);

            jnDocument!["clientId"] = clientId;
            jnDocument!["clientSecret"] = clientSecret;
        }

        step.Document = jnDocument!.ToJsonString();

        return step;
    }
}
