// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Contexts;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Services;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Database.Queries;

public class GetInstanceByIdQuery
{
    private readonly IDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public GetInstanceByIdQuery(IDbContext context, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public Instance Execute(int docId)
    {
        var instance = _context.Instances
            .SingleOrDefault(app => app.DocId == docId);

        if (instance == null)
        {
            throw new NotFoundException<int>("instance", docId);
        }

        JsonNode? jnDocument = JsonNode.Parse(instance.Document);

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

        instance.Document = jnDocument!.ToJsonString();

        return instance;
    }
}