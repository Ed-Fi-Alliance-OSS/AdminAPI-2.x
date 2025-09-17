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
public class EditOdsInstanceContextCommandTests
{
    private IUsersContext _usersContext;
    private EditOdsInstanceContextCommand _command;
    private DbSet<OdsInstance> _odsInstances;
    private DbSet<OdsInstanceContext> _odsInstanceContexts;

    [SetUp]
    public void SetUp()
    {
        _usersContext = A.Fake<IUsersContext>();
        _odsInstances = A.Fake<DbSet<OdsInstance>>();
        _odsInstanceContexts = A.Fake<DbSet<OdsInstanceContext>>();
        
        A.CallTo(() => _usersContext.OdsInstances).Returns(_odsInstances);
        A.CallTo(() => _usersContext.OdsInstanceContexts).Returns(_odsInstanceContexts);
        
        _command = new EditOdsInstanceContextCommand(_usersContext);
    }

    [Test]
    public void Execute_WithValidModel_UpdatesAndReturnsOdsInstanceContext()
    {
        // Arrange
        var existingOdsInstance = new OdsInstance { OdsInstanceId = 1, Name = "Original Instance" };
        var newOdsInstance = new OdsInstance { OdsInstanceId = 2, Name = "New Instance" };
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "OriginalKey",
            ContextValue = "OriginalValue",
            OdsInstance = existingOdsInstance
        };

        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(1);
        A.CallTo(() => model.OdsInstanceId).Returns(2);
        A.CallTo(() => model.ContextKey).Returns("UpdatedKey");
        A.CallTo(() => model.ContextValue).Returns("UpdatedValue");

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstances.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstance, bool>>>.Ignored))
            .Returns(newOdsInstance);

        // Act
        var result = _command.Execute(model);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(existingContext);
        result.ContextKey.ShouldBe("UpdatedKey");
        result.ContextValue.ShouldBe("UpdatedValue");
        result.OdsInstance.ShouldBe(newOdsInstance);
        
        A.CallTo(() => _usersContext.SaveChanges()).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void Execute_WhenOdsInstanceContextNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(999);
        A.CallTo(() => model.OdsInstanceId).Returns(1);
        A.CallTo(() => model.ContextKey).Returns("TestKey");
        A.CallTo(() => model.ContextValue).Returns("TestValue");

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(null);

        // Act & Assert
        var exception = Should.Throw<NotFoundException<int>>(() => _command.Execute(model));
        exception.ResourceName.ShouldBe("odsInstanceContext");
        exception.Id.ShouldBe(999);
    }

    [Test]
    public void Execute_WhenOdsInstanceNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "OriginalKey",
            ContextValue = "OriginalValue"
        };

        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(1);
        A.CallTo(() => model.OdsInstanceId).Returns(999);
        A.CallTo(() => model.ContextKey).Returns("TestKey");
        A.CallTo(() => model.ContextValue).Returns("TestValue");

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstances.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstance, bool>>>.Ignored))
            .Returns(null);

        // Act & Assert
        var exception = Should.Throw<NotFoundException<int>>(() => _command.Execute(model));
        exception.ResourceName.ShouldBe("odsInstance");
        exception.Id.ShouldBe(999);
    }

    [Test]
    public void Execute_WithNullContextKey_UpdatesContextWithNullKey()
    {
        // Arrange
        var existingOdsInstance = new OdsInstance { OdsInstanceId = 1, Name = "Test Instance" };
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "OriginalKey",
            ContextValue = "OriginalValue",
            OdsInstance = existingOdsInstance
        };

        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(1);
        A.CallTo(() => model.OdsInstanceId).Returns(1);
        A.CallTo(() => model.ContextKey).Returns(null);
        A.CallTo(() => model.ContextValue).Returns("UpdatedValue");

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstances.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstance, bool>>>.Ignored))
            .Returns(existingOdsInstance);

        // Act
        var result = _command.Execute(model);

        // Assert
        result.ShouldNotBeNull();
        result.ContextKey.ShouldBeNull();
        result.ContextValue.ShouldBe("UpdatedValue");
        result.OdsInstance.ShouldBe(existingOdsInstance);
    }

    [Test]
    public void Execute_WithNullContextValue_UpdatesContextWithNullValue()
    {
        // Arrange
        var existingOdsInstance = new OdsInstance { OdsInstanceId = 1, Name = "Test Instance" };
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "OriginalKey",
            ContextValue = "OriginalValue",
            OdsInstance = existingOdsInstance
        };

        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(1);
        A.CallTo(() => model.OdsInstanceId).Returns(1);
        A.CallTo(() => model.ContextKey).Returns("UpdatedKey");
        A.CallTo(() => model.ContextValue).Returns(null);

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstances.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstance, bool>>>.Ignored))
            .Returns(existingOdsInstance);

        // Act
        var result = _command.Execute(model);

        // Assert
        result.ShouldNotBeNull();
        result.ContextKey.ShouldBe("UpdatedKey");
        result.ContextValue.ShouldBeNull();
        result.OdsInstance.ShouldBe(existingOdsInstance);
    }

    [Test]
    public void Execute_WhenSaveChangesFails_ExceptionIsPropagated()
    {
        // Arrange
        var existingOdsInstance = new OdsInstance { OdsInstanceId = 1, Name = "Test Instance" };
        var existingContext = new OdsInstanceContext 
        { 
            OdsInstanceContextId = 1,
            ContextKey = "OriginalKey",
            ContextValue = "OriginalValue",
            OdsInstance = existingOdsInstance
        };

        var model = A.Fake<IEditOdsInstanceContextModel>();
        A.CallTo(() => model.Id).Returns(1);
        A.CallTo(() => model.OdsInstanceId).Returns(1);
        A.CallTo(() => model.ContextKey).Returns("UpdatedKey");
        A.CallTo(() => model.ContextValue).Returns("UpdatedValue");

        A.CallTo(() => _odsInstanceContexts.Include(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, object>>>.Ignored))
            .Returns(_odsInstanceContexts);
        A.CallTo(() => _odsInstanceContexts.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstanceContext, bool>>>.Ignored))
            .Returns(existingContext);
        A.CallTo(() => _odsInstances.SingleOrDefault(A<System.Linq.Expressions.Expression<System.Func<OdsInstance, bool>>>.Ignored))
            .Returns(existingOdsInstance);
        A.CallTo(() => _usersContext.SaveChanges()).Throws(new System.Exception("Database error"));

        // Act & Assert
        Should.Throw<System.Exception>(() => _command.Execute(model));
    }

    [Test]
    public void Constructor_WithValidContext_InitializesSuccessfully()
    {
        // Arrange & Act
        var command = new EditOdsInstanceContextCommand(_usersContext);

        // Assert
        command.ShouldNotBeNull();
    }

    [Test]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<System.ArgumentNullException>(() => new EditOdsInstanceContextCommand(null));
    }
}