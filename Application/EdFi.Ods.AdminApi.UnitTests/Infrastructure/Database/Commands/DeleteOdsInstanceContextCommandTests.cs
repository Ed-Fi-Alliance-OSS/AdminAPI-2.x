// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Infrastructure.Database.Commands;

[TestFixture]
public class DeleteOdsInstanceContextCommandTests
{
    private IUsersContext _usersContext;
    private DeleteOdsInstanceContextCommand _command;
    private DbSet<OdsInstanceContext> _odsInstanceContexts;

    [SetUp]
    public void SetUp()
    {
        _usersContext = A.Fake<IUsersContext>();
        _odsInstanceContexts = A.Fake<DbSet<OdsInstanceContext>>();
        
        A.CallTo(() => _usersContext.OdsInstanceContexts).Returns(_odsInstanceContexts);
        
        _command = new DeleteOdsInstanceContextCommand(_usersContext);
    }

    [Test]
    public void Execute_WithValidId_RemovesOdsInstanceContext()
    {
        // Arrange
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);

        // Act
        _command.Execute(1);

        // Assert
        A.CallTo(() => _odsInstanceContexts.Remove(existingContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _usersContext.SaveChanges()).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Execute_WhenOdsInstanceContextNotFound_ThrowsNotFoundException()
    {
        // Arrange
        int nonExistentId = 999;

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(null);

        // Act & Assert
        var exception = Should.Throw<NotFoundException<int>>(() => _command.Execute(nonExistentId));
        exception.ResourceName.ShouldBe("odsInstanceContext");
        exception.Id.ShouldBe(nonExistentId);
    }

    [Test]
    public void Execute_WithZeroId_ThrowsNotFoundException()
    {
        // Arrange
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(null);

        // Act & Assert
        var exception = Should.Throw<NotFoundException<int>>(() => _command.Execute(0));
        exception.ResourceName.ShouldBe("odsInstanceContext");
        exception.Id.ShouldBe(0);
    }

    [Test]
    public void Execute_WithNegativeId_ThrowsNotFoundException()
    {
        // Arrange
        int negativeId = -1;

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(null);

        // Act & Assert
        var exception = Should.Throw<NotFoundException<int>>(() => _command.Execute(negativeId));
        exception.ResourceName.ShouldBe("odsInstanceContext");
        exception.Id.ShouldBe(negativeId);
    }

    [Test]
    public void Execute_WhenRemoveSucceeds_CallsSaveChanges()
    {
        // Arrange
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 123,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);

        // Act
        _command.Execute(123);

        // Assert
        A.CallTo(() => _odsInstanceContexts.Remove(existingContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _usersContext.SaveChanges()).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Execute_WhenSaveChangesFails_ExceptionIsPropagated()
    {
        // Arrange
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _usersContext.SaveChanges()).Throws(new System.Exception("Database error"));

        // Act & Assert
        Should.Throw<System.Exception>(() => _command.Execute(1));
    }

    [Test]
    public void Execute_WhenRemoveFails_ExceptionIsPropagated()
    {
        // Arrange
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };

        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstanceContexts.Remove(existingContext)).Throws(new System.Exception("Remove failed"));

        // Act & Assert
        Should.Throw<System.Exception>(() => _command.Execute(1));
    }

    [Test]
    public void Constructor_WithValidContext_InitializesSuccessfully()
    {
        // Arrange & Act
        var command = new DeleteOdsInstanceContextCommand(_usersContext);

        // Assert
        command.ShouldNotBeNull();
    }

    [Test]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<System.ArgumentNullException>(() => new DeleteOdsInstanceContextCommand(null));
    }
}