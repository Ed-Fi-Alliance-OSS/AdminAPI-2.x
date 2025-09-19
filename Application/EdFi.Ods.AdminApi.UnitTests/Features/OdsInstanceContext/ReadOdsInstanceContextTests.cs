// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Features.OdsInstanceContext;

[TestFixture]
public class ReadOdsInstanceContextTests
{
    [Test]
    public void ReadOdsInstanceContext_ImplementsIFeature()
    {
        // Arrange & Act
        var readOdsInstanceContext = new ReadOdsInstanceContext();

        // Assert
        readOdsInstanceContext.ShouldBeAssignableTo<EdFi.Ods.AdminApi.Common.Features.IFeature>();
    }

    [Test]
    public void MapEndpoints_DoesNotThrow()
    {
        // Arrange
        var readOdsInstanceContext = new ReadOdsInstanceContext();

        // Act & Assert
        Should.NotThrow(() => readOdsInstanceContext.MapEndpoints(null!));
    }
}