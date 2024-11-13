// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Queries;

public interface IGetStepsQuery
{
    Task<IEnumerable<Step>> Execute();
}

public class GetStepsQuery : IGetStepsQuery
{
    private readonly IQueriesRepository<Step> _stepQuery;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetStepsQuery(IQueriesRepository<Step> stepQuery, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _stepQuery = stepQuery;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }
    public async Task<IEnumerable<Step>> Execute()
    {
        var steps = await _stepQuery.GetAllAsync();

        foreach (var step in steps)
        {
            JsonNode? jn = JsonNode.Parse(step.Document);

            var encryptedClientId = jn!["clientId"]?.AsValue().ToString();
            var encryptedClientSecret = jn!["clientSecret"]?.AsValue().ToString();

            var clientId = string.Empty;
            var clientSecret = string.Empty;

            if (!string.IsNullOrEmpty(encryptedClientId) && !string.IsNullOrEmpty(encryptedClientSecret))
            {
                _encryptionService.TryDecrypt(encryptedClientId, _encryptionKey, out clientId);
                _encryptionService.TryDecrypt(encryptedClientSecret, _encryptionKey, out clientSecret);

                jn!["clientId"] = clientId;
                jn!["clientSecret"] = clientSecret;
            }

            step.Document = jn!.ToJsonString();
        }

        return steps.ToList();
    }
}
