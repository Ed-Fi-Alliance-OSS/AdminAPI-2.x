// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Services;
using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class AddInstanceCommandTests : PlatformUsersContextTestBase
{
    private IOptions<AppSettings> _options { get; set; }

    [OneTimeSetUp]
    public virtual async Task FixtureSetup()
    {
        AdminConsoleSettings appSettings = new AdminConsoleSettings();
        await Task.Yield();
    }

    [Test]
    public void ShouldExecute()
    {
        var instanceDocument = "{\"instanceId\":\"DEF456\",\"tenantId\":\"def456\",\"instanceName\":\"Mock Instance 2\",\"instanceType\":\"Type B\",\"connectionType\":\"Type Y\",\"clientId\":\"CLIENT321\",\"clientSecret\":\"SECRET456\",\"baseUrl\":\"https://localhost/api\",\"authenticationUrl\":\"https://localhost/api/oauth/token\",\"resourcesUrl\":\"https://localhost/api\",\"schoolYears\":[2024,2025],\"isDefault\":false,\"verificationStatus\":null,\"provider\":\"Local\"}";
        AddInstanceResult result = null;

        var encryptionService = new EncryptionService();
        var encryptionKey = Testing.GetEncryptionKeyResolver().GetEncryptionKey();

        Transaction(dbContext =>
        {
            var command = new AddInstanceCommand(dbContext, Testing.GetEncryptionKeyResolver(), encryptionService);
            var newInstance = new TestInstance
            {
                InstanceId = 1,
                DocId = null,
                EdOrgId = 1,
                Document = instanceDocument
            };

            result = command.Execute(newInstance);
        });

        Transaction(dbContext =>
        {
            var persistedInstance = dbContext.Instances;
            persistedInstance.Count().ShouldBe(1);
            persistedInstance.First().DocId.ShouldNotBeNull();
            persistedInstance.First().InstanceId.ShouldBe(1);
            persistedInstance.First().EdOrgId.ShouldBe(1);

            JsonNode jnDocument = JsonNode.Parse(persistedInstance.First().Document);

            var encryptedClientId = jnDocument!["clientId"]?.AsValue().ToString();
            var encryptedClientSecret = jnDocument!["clientSecret"]?.AsValue().ToString();

            var clientId = "CLIENT321";
            var clientSecret = "SECRET456";

            if (!string.IsNullOrEmpty(encryptedClientId) && !string.IsNullOrEmpty(encryptedClientSecret))
            {
                encryptionService.TryDecrypt(encryptedClientId, encryptionKey, out clientId);
                encryptionService.TryDecrypt(encryptedClientSecret, encryptionKey, out clientSecret);

                clientId.ShouldBe(clientId);
                clientSecret.ShouldBe(clientSecret);
            }
        });
    }

    private class TestInstance : IAddInstanceModel
    {
        public int? DocId { get; set; }
        public int? InstanceId { get; set; }
        public int? EdOrgId { get; set; }
        public string Document { get; set; }
    }
}