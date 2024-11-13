// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repository;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Commands;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Steps.Queries;
using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests;

[TestFixture]
public class GetStepByIdQueryTests : PlatformUsersContextTestBase
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
        Step result = null;

        var newStep = new TestStep
        {
            InstanceId = 1,
            TenantId = 1,
            EdOrgId = 1,
            Document = stepDocument
        };

        Transaction(async dbContext =>
        {
            var repository = new CommandRepository<Step>(dbContext);
            var command = new AddStepCommand(repository, Testing.GetEncryptionKeyResolver(), new EncryptionService());

            result = await command.Execute(newStep);
        });

        Transaction(async dbContext =>
        {
            var repository = new QueriesRepository<Step>(dbContext);
            var query = new GetStepQuery(repository, Testing.GetEncryptionKeyResolver(), new EncryptionService());
            var step = await query.Execute(result.DocId.Value);

            step.DocId.ShouldBe(result.DocId);
            step.TenantId.ShouldBe(newStep.TenantId);
            step.InstanceId.ShouldBe(newStep.InstanceId);
            step.EdOrgId.ShouldBe(newStep.EdOrgId);
            step.Document.ShouldBe(newStep.Document);
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
