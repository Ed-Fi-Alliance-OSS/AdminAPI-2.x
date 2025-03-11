// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using Results = FluentValidation.Results;

namespace EdFi.Ods.AdminApi.Common.UnitTests.Features;

[TestFixture]
public class AdminApiErrorTest
{
    [Test]
    public void Validation_ShouldReturnValidationProblem()
    {
        // Arrange
        var validationFailures = new List<Results.ValidationFailure>
        {
            new("Property1", "Error1"),
            new("Property1", "Error2"),
            new("Property2", "Error3")
        };

        // Act
        var result = AdminApiError.Validation(validationFailures);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var validationProblemDetails = result as ProblemHttpResult;
        validationProblemDetails.ShouldNotBeNull();

        var details = validationProblemDetails.ProblemDetails as HttpValidationProblemDetails;
        details.ShouldNotBeNull();

        details.Errors.ShouldContainKey("Property1");
        details.Errors["Property1"].ShouldBeEquivalentTo(new[] { "Error1", "Error2" });
        details.Errors.ShouldContainKey("Property2");
        details.Errors["Property2"].ShouldBeEquivalentTo(new[] { "Error3" });
    }

    [Test]
    public void Unexpected_ShouldReturnProblemWithMessage()
    {
        // Arrange
        var message = "Unexpected error occurred";

        // Act
        var result = AdminApiError.Unexpected(message);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problemDetails = result as ProblemHttpResult;
        problemDetails.ShouldNotBeNull();

        problemDetails.ProblemDetails.Title.ShouldBe(message);
        problemDetails.ProblemDetails.Status.ShouldBe(500);
    }

    [Test]
    public void Unexpected_WithErrors_ShouldReturnProblemWithMessageAndErrors()
    {
        // Arrange
        var message = "Unexpected error occurred";
        var errors = new[] { "Error1", "Error2" };

        // Act
        var result = AdminApiError.Unexpected(message, errors);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problemDetails = result as ProblemHttpResult;
        problemDetails.ShouldNotBeNull();

        problemDetails.ProblemDetails.Title.ShouldBe(message);
        problemDetails.ProblemDetails.Status.ShouldBe(500);
        problemDetails.ProblemDetails.Extensions["errors"].ShouldBeEquivalentTo(errors);
    }

    [Test]
    public void Unexpected_WithException_ShouldReturnProblemWithExceptionMessage()
    {
        // Arrange
        var exception = new Exception("Exception message");

        // Act
        var result = AdminApiError.Unexpected(exception);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problemDetails = result as ProblemHttpResult;
        problemDetails.ShouldNotBeNull();

        problemDetails.ProblemDetails.Title.ShouldBe(exception.Message);
        problemDetails.ProblemDetails.Status.ShouldBe(500);
    }

    [Test]
    public void NotFound_ShouldReturnNotFoundWithResourceNameAndId()
    {
        // Arrange
        var resourceName = "Resource";
        var id = 123;

        // Act
        var result = AdminApiError.NotFound(resourceName, id);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = result as NotFound<string>;
        notFoundResult.ShouldNotBeNull();

        notFoundResult.Value.ShouldBe($"Not found: {resourceName} with ID {id}");
    }

    [Test]
    public void Validation_WithEmptyErrors_ShouldReturnValidationProblem()
    {
        // Act
        var result = AdminApiError.Validation([]);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var validationProblemDetails = result as ProblemHttpResult;
        validationProblemDetails.ShouldNotBeNull();

        var details = validationProblemDetails.ProblemDetails as HttpValidationProblemDetails;
        details.ShouldNotBeNull();

        details.Errors.ShouldBeEmpty();
    }
}
