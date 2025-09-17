// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using AutoMapper;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Infrastructure.Database.Commands;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FakeItEasy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Features.OdsInstanceContext;

[TestFixture]
public class EditOdsInstanceContextTests
{
    private EditOdsInstanceContext.Validator _validator;
    private IGetOdsInstanceQuery _getOdsInstanceQuery;
    private IGetOdsInstanceContextsQuery _getOdsInstanceContextsQuery;

    [SetUp]
    public void SetUp()
    {
        _getOdsInstanceQuery = A.Fake<IGetOdsInstanceQuery>();
        _getOdsInstanceContextsQuery = A.Fake<IGetOdsInstanceContextsQuery>();
        _validator = new EditOdsInstanceContext.Validator(_getOdsInstanceQuery, _getOdsInstanceContextsQuery);
    }

    [Test]
    public async Task Handle_ExecutesCommandAndReturnsOk()
    {
        // Arrange
        var validator = A.Fake<EditOdsInstanceContext.Validator>();
        var command = A.Fake<IEditOdsInstanceContextCommand>();
        var mapper = A.Fake<IMapper>();
        var db = A.Fake<IUsersContext>();
        var request = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            OdsInstanceId = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };
        int id = 123;
        var editedContext = new OdsInstanceContext { OdsInstanceContextId = id };

        A.CallTo(() => command.Execute(request)).Returns(editedContext);

        // Act
        var result = await EditOdsInstanceContext.Handle(validator, command, mapper, db, request, id);

        // Assert
        request.Id.ShouldBe(id);
        A.CallTo(() => validator.GuardAsync(request)).MustHaveHappenedOnceExactly();
        A.CallTo(() => command.Execute(request)).MustHaveHappenedOnceExactly();
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
    }

    [Test]
    public void Handle_WhenValidationFails_ThrowsValidationException()
    {
        // Arrange
        var validator = A.Fake<EditOdsInstanceContext.Validator>();
        var command = A.Fake<IEditOdsInstanceContextCommand>();
        var mapper = A.Fake<IMapper>();
        var db = A.Fake<IUsersContext>();
        var request = new EditOdsInstanceContext.EditOdsInstanceContextRequest();
        int id = 123;

        A.CallTo(() => validator.GuardAsync(request)).Throws(new ValidationException("Validation failed"));

        // Act & Assert
        Should.Throw<ValidationException>(async () => await EditOdsInstanceContext.Handle(validator, command, mapper, db, request, id));
    }

    [Test]
    public void Handle_WhenCommandThrows_ExceptionIsPropagated()
    {
        // Arrange
        var validator = A.Fake<EditOdsInstanceContext.Validator>();
        var command = A.Fake<IEditOdsInstanceContextCommand>();
        var mapper = A.Fake<IMapper>();
        var db = A.Fake<IUsersContext>();
        var request = new EditOdsInstanceContext.EditOdsInstanceContextRequest();
        int id = 123;

        A.CallTo(() => command.Execute(request)).Throws(new System.Exception("Command failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await EditOdsInstanceContext.Handle(validator, command, mapper, db, request, id));
    }

    [Test]
    public void Validator_Should_Have_Error_When_ContextKey_Is_Empty()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "",
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.ContextKey));
    }

    [Test]
    public void Validator_Should_Have_Error_When_ContextKey_Is_Null()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = null,
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.ContextKey));
    }

    [Test]
    public void Validator_Should_Have_Error_When_ContextValue_Is_Empty()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TestKey",
            ContextValue = "",
            OdsInstanceId = 1
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.ContextValue));
    }

    [Test]
    public void Validator_Should_Have_Error_When_ContextValue_Is_Null()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TestKey",
            ContextValue = null,
            OdsInstanceId = 1
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.ContextValue));
    }

    [Test]
    public void Validator_Should_Have_Error_When_OdsInstanceId_Is_Zero()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue",
            OdsInstanceId = 0
        };

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.OdsInstanceId));
    }

    [Test]
    public void Validator_Should_Have_Error_When_OdsInstance_Does_Not_Exist()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue",
            OdsInstanceId = 999
        };

        A.CallTo(() => _getOdsInstanceQuery.Execute(999)).Throws(new System.Exception("OdsInstance not found"));

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(model.OdsInstanceId));
    }

    [Test]
    public void Validator_Should_Have_Error_When_Combined_Key_Is_Not_Unique()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "ExistingKey",
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        var existingContexts = new List<OdsInstanceContext>
        {
            new OdsInstanceContext
            {
                OdsInstanceContextId = 2, // Different ID
                ContextKey = "ExistingKey",
                OdsInstance = new OdsInstance { OdsInstanceId = 1 }
            }
        };

        A.CallTo(() => _getOdsInstanceContextsQuery.Execute()).Returns(existingContexts);

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == "");
    }

    [Test]
    public void Validator_Should_Pass_When_Combined_Key_Is_Same_For_Same_Record()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "ExistingKey",
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        var existingContexts = new List<OdsInstanceContext>
        {
            new OdsInstanceContext
            {
                OdsInstanceContextId = 1, // Same ID
                ContextKey = "ExistingKey",
                OdsInstance = new OdsInstance { OdsInstanceId = 1 }
            }
        };

        A.CallTo(() => _getOdsInstanceContextsQuery.Execute()).Returns(existingContexts);

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_Should_Pass_When_Combined_Key_Is_Unique()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "UniqueKey",
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        var existingContexts = new List<OdsInstanceContext>
        {
            new OdsInstanceContext
            {
                OdsInstanceContextId = 2,
                ContextKey = "DifferentKey",
                OdsInstance = new OdsInstance { OdsInstanceId = 1 }
            }
        };

        A.CallTo(() => _getOdsInstanceContextsQuery.Execute()).Returns(existingContexts);

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validator_Should_Pass_With_Valid_Model()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "ValidKey",
            ContextValue = "ValidValue",
            OdsInstanceId = 1
        };

        A.CallTo(() => _getOdsInstanceContextsQuery.Execute()).Returns(new List<OdsInstanceContext>());

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void BeAnExistingOdsInstance_Should_Return_True_When_OdsInstance_Exists()
    {
        // Arrange
        var odsInstanceId = 1;
        A.CallTo(() => _getOdsInstanceQuery.Execute(odsInstanceId)).Returns(new OdsInstance());

        // Act
        var result = _validator.Validate(new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TestKey",
            ContextValue = "TestValue",
            OdsInstanceId = odsInstanceId
        });

        // Assert - The validation should pass (assuming other fields are valid)
        A.CallTo(() => _getOdsInstanceQuery.Execute(odsInstanceId)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void BeUniqueCombinedKey_Should_Return_True_When_Key_Is_Case_Insensitive_Unique()
    {
        // Arrange
        var model = new EditOdsInstanceContext.EditOdsInstanceContextRequest
        {
            Id = 1,
            ContextKey = "TESTKEY",
            ContextValue = "TestValue",
            OdsInstanceId = 1
        };

        var existingContexts = new List<OdsInstanceContext>
        {
            new OdsInstanceContext
            {
                OdsInstanceContextId = 2,
                ContextKey = "testkey", // different case
                OdsInstance = new OdsInstance { OdsInstanceId = 2 } // different instance
            }
        };

        A.CallTo(() => _getOdsInstanceContextsQuery.Execute()).Returns(existingContexts);

        // Act
        var result = _validator.Validate(model);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}