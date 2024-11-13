// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Commands;
using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class AddStepCommandTests : PlatformUsersContextTestBase
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
        var stepDocument = "{\"data\":[],\"type\":\"Response\"}";

        var encryptionService = new EncryptionService();
        var encryptionKey = Testing.GetEncryptionKeyResolver().GetEncryptionKey();

        Transaction(async dbContext =>
        {
            var repository = new CommandRepository<Step>(dbContext);
            var newStep = new TestStep
            {
                InstanceId = 1,
                TenantId = 1,
                EdOrgId = 1,
                Document = stepDocument
            };

            var command = new AddStepCommand(repository, Testing.GetEncryptionKeyResolver(), encryptionService);

            var result = await command.Execute(newStep);
        });

        Transaction(dbContext =>
        {
            var persistedStep = dbContext.Steps;
            persistedStep.Count().ShouldBe(1);
            persistedStep.First().DocId.ShouldBe(1);
            persistedStep.First().TenantId.ShouldBe(1);
            persistedStep.First().InstanceId.ShouldBe(1);
            persistedStep.First().EdOrgId.ShouldBe(1);

            JsonNode jnDocument = JsonNode.Parse(persistedStep.First().Document);

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

    private class TestStep : IAddStepModel
    {
        public int DocId { get; }
        public int TenantId { get; set; }
        public int InstanceId { get; set; }
        public int? EdOrgId { get; set; }
        public string Document { get; set; }
    }
}
