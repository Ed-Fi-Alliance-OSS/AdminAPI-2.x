// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Dynamic;
using System.Text.Json.Nodes;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstanceContexts;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstances;
using EdFi.Ods.AdminApi.Common.Infrastructure.Database.Services.OdsInstanceDerivatives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;

public interface IAddInstanceCommand
{
    Task<Instance> Execute(IAddInstanceModel instance);
}

public class AddInstanceCommand : IAddInstanceCommand
{
    private readonly ICommandRepository<Instance> _instanceCommand;
    private readonly IOdsInstancesHandler _odsInstancesHandler;
    private readonly IOdsInstanceContextsHandler _odsInstanceContextsHandler;
    private readonly IOdsInstanceDerivativesHandler _odsInstanceDerivativesHandler;
    private readonly IEncryptionService _encryptionService;
    private readonly string _encryptionKey;

    public AddInstanceCommand(ICommandRepository<Instance> instanceCommand, IOdsInstancesHandler odsInstancesHandler, IOdsInstanceContextsHandler odsInstanceContextsHandler,
       IOdsInstanceDerivativesHandler odsInstanceDerivativesHandler, IEncryptionKeyResolver encryptionKeyResolver, IEncryptionService encryptionService)
    {
        _instanceCommand = instanceCommand;
        _odsInstancesHandler = odsInstancesHandler;
        _odsInstanceContextsHandler = odsInstanceContextsHandler;
        _odsInstanceDerivativesHandler = odsInstanceDerivativesHandler;
        _encryptionKey = encryptionKeyResolver.GetEncryptionKey();
        _encryptionService = encryptionService;
    }

    public async Task<Instance> Execute(IAddInstanceModel instance)
    {
        JsonNode? jnDocument = ExpandoObjectHelper.FormatJson(instance.Document);

        var clientId = jnDocument!["clientId"]?.AsValue().ToString();
        var clientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

        var encryptedClientId = string.Empty;
        var encryptedClientSecret = string.Empty;

        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            _encryptionService.TryEncrypt(clientId, _encryptionKey, out encryptedClientId);
            _encryptionService.TryEncrypt(clientSecret, _encryptionKey, out encryptedClientSecret);

            jnDocument!["clientId"] = encryptedClientId;
            jnDocument!["clientSecret"] = encryptedClientSecret;
        }

        //Handle OdsInstance
        var resultingOdsInstanceId = _odsInstancesHandler.HandleOdsInstance(instance.OdsInstanceId, jnDocument);

        //Handle OdsInstanceContexts if present in payload
        var odsInstanceContextsString = jnDocument["odsInstanceContexts"].ToString();
        if (!string.IsNullOrEmpty(odsInstanceContextsString))
        {
            var odsInstanceContexts = System.Text.Json.JsonSerializer.Deserialize<JsonArray>(odsInstanceContextsString);
            foreach (var odsInstanceContext in odsInstanceContexts)
            {
                odsInstanceContext["odsInstanceId"] = resultingOdsInstanceId;
                _odsInstanceContextsHandler.HandleOdsInstanceContexts(odsInstanceContext!);
            }
        }
        //Handle OdsInstanceDerivatives if present in payload
        var odsInstanceDerivativesString = jnDocument["odsInstanceDerivatives"].ToString();
        if (!string.IsNullOrEmpty(odsInstanceDerivativesString))
        {
            var odsInstanceDerivatives = System.Text.Json.JsonSerializer.Deserialize<JsonArray>(odsInstanceDerivativesString);
            foreach (var odsInstanceDerivative in odsInstanceDerivatives)
            {
                odsInstanceDerivative["odsInstanceId"] = resultingOdsInstanceId;
                _odsInstanceDerivativesHandler.HandleOdsInstanceDerivatives(odsInstanceDerivative!);
            }
        }

        try
        {
            return await _instanceCommand.AddAsync(new Instance
            {
                OdsInstanceId = resultingOdsInstanceId ?? instance.OdsInstanceId,
                TenantId = instance.TenantId,
                EdOrgId = instance.EdOrgId,
                Document = jnDocument!.ToJsonString(),
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
}

public interface IAddInstanceModel
{
    int OdsInstanceId { get; }
    int? EdOrgId { get; }
    int TenantId { get; }
    ExpandoObject Document { get; }
}

public class AddInstanceResult
{
    public int DocId { get; set; }
}
