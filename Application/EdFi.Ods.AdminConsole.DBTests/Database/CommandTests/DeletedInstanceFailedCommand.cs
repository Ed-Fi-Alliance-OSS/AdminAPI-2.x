// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Contexts.Admin.MsSql;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.DataAccess.Models;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services.Instances.Commands;
using EdFi.Ods.AdminConsole.DBTests.Common;
using NUnit.Framework;

namespace EdFi.Ods.AdminConsole.DBTests.Database.CommandTests
{
    [TestFixture]
    public class DeletedInstanceFailedCommand : PlatformUsersContextTestBase
    {
        [Test]
        public async Task ShouldSetDeletePendingStatusInInstance()
        {
            var newInstanceId = 0;
            var dbContext = new AdminConsoleMsSqlContext(GetDbContextOptions());

            var repository = new CommandRepository<Instance>(dbContext);
            var addCommand = new AddInstanceCommand(repository);
            var result = await addCommand.Execute(new TestInstance
            {
                TenantId = 1,
                OdsInstanceId = 1,
                Name = "Test Complete Instance",
                InstanceType = "Standard",
                Status = InstanceStatus.Completed.ToString(),
            });

            newInstanceId = result.Id;

            repository = new CommandRepository<Instance>(dbContext);
            var qRepository = new QueriesRepository<Instance>(dbContext);

            var command = new PendingDeleteInstanceCommand(qRepository, repository);
            await command.Execute(newInstanceId);

            var pendingDeleteResult = dbContext.Instances.FirstOrDefault(p => p.Id == newInstanceId);
            pendingDeleteResult.ShouldNotBeNull();
            pendingDeleteResult.Status.ShouldBe(InstanceStatus.Pending_Delete);
        }
    }
}
