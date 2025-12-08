// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Features.OdsInstanceContext;

[TestFixture]
public class DeleteOdsInstanceContextTests
{
    [Test]
    public async Task Handle_ExecutesDeleteCommandAndReturnsOk()
    {
        // Arrange
        var fakeCommand = A.Fake<IDeleteOdsInstanceContextCommand>();
        int testId = 123;

        // Act
        var result = await DeleteOdsInstanceContext.Handle(fakeCommand, testId);

        // Assert
        A.CallTo(() => fakeCommand.Execute(testId)).MustHaveHappenedOnceExactly();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    [Test]
    public void Handle_WhenCommandThrows_ExceptionIsPropagated()
    {
        // Arrange
        var fakeCommand = A.Fake<IDeleteOdsInstanceContextCommand>();
        int testId = 999;
        A.CallTo(() => fakeCommand.Execute(testId)).Throws(new System.Exception("Delete failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await DeleteOdsInstanceContext.Handle(fakeCommand, testId));
    }

    [Test]
    public async Task Handle_WithZeroId_ExecutesCommand()
    {
        // Arrange
        var fakeCommand = A.Fake<IDeleteOdsInstanceContextCommand>();
        int testId = 0;

        // Act
        var result = await DeleteOdsInstanceContext.Handle(fakeCommand, testId);

        // Assert
        A.CallTo(() => fakeCommand.Execute(testId)).MustHaveHappenedOnceExactly();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    [Test]
    public async Task Handle_WithNegativeId_ExecutesCommand()
    {
        // Arrange
        var fakeCommand = A.Fake<IDeleteOdsInstanceContextCommand>();
        int testId = -1;

        // Act
        var result = await DeleteOdsInstanceContext.Handle(fakeCommand, testId);

        // Assert
        A.CallTo(() => fakeCommand.Execute(testId)).MustHaveHappenedOnceExactly();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }
}