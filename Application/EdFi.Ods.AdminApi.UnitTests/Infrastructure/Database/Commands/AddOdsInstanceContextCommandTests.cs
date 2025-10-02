// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FakeItEasy;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Infrastructure.Database.Commands;

[TestFixture]
public class AddOdsInstanceContextCommandTests
{
    [Test]
    public void Constructor_WithValidContext_InitializesSuccessfully()
    {
        // Arrange & Act
        var context = A.Fake<IUsersContext>();
        var command = new AddOdsInstanceContextCommand(context);

        // Assert
        command.ShouldNotBeNull();
    }

    [Test]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<System.ArgumentNullException>(() => new AddOdsInstanceContextCommand(null));
    }
}
