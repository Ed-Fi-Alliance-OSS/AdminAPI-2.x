// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Features.OdsInstanceContext;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.UnitTests.Features.OdsInstanceContext;

[TestFixture]
public class ReadOdsInstanceContextTests
{
    [Test]
    public async Task GetOdsInstanceContexts_ReturnsOkWithMappedList()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextsQuery>();
        var fakeMapper = A.Fake<IMapper>();
        var commonQueryParams = new CommonQueryParams();
        var queryResult = new List<OdsInstanceContext> 
        { 
            new OdsInstanceContext 
            { 
                OdsInstanceContextId = 1,
                ContextKey = "Key1",
                ContextValue = "Value1"
            } 
        };
        var mappedResult = new List<OdsInstanceContextModel> 
        { 
            new OdsInstanceContextModel 
            { 
                OdsInstanceContextId = 1,
                ContextKey = "Key1",
                ContextValue = "Value1"
            } 
        };

        A.CallTo(() => fakeQuery.Execute(commonQueryParams)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<List<OdsInstanceContextModel>>(queryResult)).Returns(mappedResult);

        // Act
        var result = await ReadOdsInstanceContext.GetOdsInstanceContexts(fakeQuery, fakeMapper, commonQueryParams);

        // Assert
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<OdsInstanceContextModel>>>();
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<List<OdsInstanceContextModel>>;
        okResult!.Value.ShouldBe(mappedResult);
        A.CallTo(() => fakeQuery.Execute(commonQueryParams)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeMapper.Map<List<OdsInstanceContextModel>>(queryResult)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task GetOdsInstanceContexts_WithEmptyResult_ReturnsOkWithEmptyList()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextsQuery>();
        var fakeMapper = A.Fake<IMapper>();
        var commonQueryParams = new CommonQueryParams();
        var queryResult = new List<OdsInstanceContext>();
        var mappedResult = new List<OdsInstanceContextModel>();

        A.CallTo(() => fakeQuery.Execute(commonQueryParams)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<List<OdsInstanceContextModel>>(queryResult)).Returns(mappedResult);

        // Act
        var result = await ReadOdsInstanceContext.GetOdsInstanceContexts(fakeQuery, fakeMapper, commonQueryParams);

        // Assert
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<List<OdsInstanceContextModel>>>();
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<List<OdsInstanceContextModel>>;
        okResult!.Value.ShouldNotBeNull();
        okResult.Value.Count.ShouldBe(0);
    }

    [Test]
    public void GetOdsInstanceContexts_WhenQueryThrows_ExceptionIsPropagated()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextsQuery>();
        var fakeMapper = A.Fake<IMapper>();
        var commonQueryParams = new CommonQueryParams();

        A.CallTo(() => fakeQuery.Execute(commonQueryParams)).Throws(new System.Exception("Query failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await ReadOdsInstanceContext.GetOdsInstanceContexts(fakeQuery, fakeMapper, commonQueryParams));
    }

    [Test]
    public void GetOdsInstanceContexts_WhenMapperThrows_ExceptionIsPropagated()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextsQuery>();
        var fakeMapper = A.Fake<IMapper>();
        var commonQueryParams = new CommonQueryParams();
        var queryResult = new List<OdsInstanceContext>();

        A.CallTo(() => fakeQuery.Execute(commonQueryParams)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<List<OdsInstanceContextModel>>(queryResult)).Throws(new System.Exception("Mapping failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await ReadOdsInstanceContext.GetOdsInstanceContexts(fakeQuery, fakeMapper, commonQueryParams));
    }

    [Test]
    public async Task GetOdsInstanceContext_ReturnsOkWithMappedModel()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = 7;
        var queryResult = new OdsInstanceContext
        {
            OdsInstanceContextId = id,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };
        var mappedModel = new OdsInstanceContextModel 
        { 
            OdsInstanceContextId = id,
            ContextKey = "TestKey",
            ContextValue = "TestValue"
        };

        A.CallTo(() => fakeQuery.Execute(id)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<OdsInstanceContextModel>(queryResult)).Returns(mappedModel);

        // Act
        var result = await ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id);

        // Assert
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<OdsInstanceContextModel>>();
        var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<OdsInstanceContextModel>;
        okResult!.Value.ShouldBe(mappedModel);
        A.CallTo(() => fakeQuery.Execute(id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeMapper.Map<OdsInstanceContextModel>(queryResult)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public void GetOdsInstanceContext_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = 99;

        A.CallTo(() => fakeQuery.Execute(id)).Throws(new NotFoundException<int>("odsInstanceContext", id));

        // Act & Assert
        Should.Throw<NotFoundException<int>>(() => ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id).GetAwaiter().GetResult());
    }

    [Test]
    public void GetOdsInstanceContext_WhenQueryThrows_ExceptionIsPropagated()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = 42;

        A.CallTo(() => fakeQuery.Execute(id)).Throws(new System.Exception("Query failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id));
    }

    [Test]
    public void GetOdsInstanceContext_WhenMapperThrows_ExceptionIsPropagated()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = 7;
        var queryResult = new OdsInstanceContext();

        A.CallTo(() => fakeQuery.Execute(id)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<OdsInstanceContextModel>(queryResult)).Throws(new System.Exception("Mapping failed"));

        // Act & Assert
        Should.Throw<System.Exception>(async () => await ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id));
    }

    [Test]
    public async Task GetOdsInstanceContext_WithZeroId_ExecutesQuery()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = 0;
        var queryResult = new OdsInstanceContext { OdsInstanceContextId = id };
        var mappedModel = new OdsInstanceContextModel { OdsInstanceContextId = id };

        A.CallTo(() => fakeQuery.Execute(id)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<OdsInstanceContextModel>(queryResult)).Returns(mappedModel);

        // Act
        var result = await ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id);

        // Assert
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<OdsInstanceContextModel>>();
        A.CallTo(() => fakeQuery.Execute(id)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task GetOdsInstanceContext_WithNegativeId_ExecutesQuery()
    {
        // Arrange
        var fakeQuery = A.Fake<IGetOdsInstanceContextByIdQuery>();
        var fakeMapper = A.Fake<IMapper>();
        int id = -1;
        var queryResult = new OdsInstanceContext { OdsInstanceContextId = id };
        var mappedModel = new OdsInstanceContextModel { OdsInstanceContextId = id };

        A.CallTo(() => fakeQuery.Execute(id)).Returns(queryResult);
        A.CallTo(() => fakeMapper.Map<OdsInstanceContextModel>(queryResult)).Returns(mappedModel);

        // Act
        var result = await ReadOdsInstanceContext.GetOdsInstanceContext(fakeQuery, fakeMapper, id);

        // Assert
        result.ShouldBeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok<OdsInstanceContextModel>>();
        A.CallTo(() => fakeQuery.Execute(id)).MustHaveHappenedOnceExactly();
    }
}