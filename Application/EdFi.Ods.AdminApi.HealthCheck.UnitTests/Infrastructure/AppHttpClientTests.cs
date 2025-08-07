// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using EdFi.Ods.AdminApi.HealthCheck.Infrastructure;
using EdFi.Ods.AdminApi.HealthCheck;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.HealthCheck.UnitTests.Infrastructure;

[TestFixture]
public class AppHttpClientTests
{
    private AppSettings _settings;
    private IOptions<AppSettings> _options;
    private ILogger<AppHttpClient> _logger;

    [SetUp]
    public void SetUp()
    {
        _settings = new AppSettings { MaxRetryAttempts = 2 };
        _options = Options.Create(_settings);
        _logger = A.Fake<ILogger<AppHttpClient>>();
    }

    private static HttpClient CreateHttpClient(HttpResponseMessage responseMessage, out FakeHttpMessageHandler handler)
    {
        handler = new FakeHttpMessageHandler(responseMessage);
        return new HttpClient(handler);
    }

    [Test]
    public async Task SendAsync_StringContent_ReturnsApiResponse_OnSuccess()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("success")
        };
        var httpClient = CreateHttpClient(responseMessage, out _);
        var sut = new AppHttpClient(httpClient, _logger, _options);

        // Act
        var result = await sut.SendAsync("http://test", HttpMethod.Get, new StringContent(""), null);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Content.ShouldBe("success");
    }

    [Test]
    public async Task SendAsync_StringContent_RetriesOnTransientFailure()
    {
        // Arrange
        int callCount = 0;
        var handler = new FakeHttpMessageHandler(() =>
        {
            callCount++;
            if (callCount == 1)
                return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("recovered") };
        });

        var httpClient = new HttpClient(handler);
        var sut = new AppHttpClient(httpClient, _logger, _options);

        // Act
        var result = await sut.SendAsync("http://test", HttpMethod.Get, new StringContent(""), null);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Content.ShouldBe("recovered");
        callCount.ShouldBe(2);
    }

    [Test]
    public async Task SendAsync_StringContent_LogsWarning_OnNonOkStatus()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad request")
        };
        var httpClient = CreateHttpClient(responseMessage, out _);
        var sut = new AppHttpClient(httpClient, _logger, _options);

        // Act
        var result = await sut.SendAsync("http://test", HttpMethod.Post, new StringContent(""), null);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Content.ShouldBe("bad request");
    }

    [Test]
    public async Task SendAsync_FormUrlEncodedContent_ReturnsApiResponse_OnSuccess()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("form success")
        };
        var httpClient = CreateHttpClient(responseMessage, out _);
        var sut = new AppHttpClient(httpClient, _logger, _options);

        // Act
        var result = await sut.SendAsync("http://test", HttpMethod.Post, new FormUrlEncodedContent([]), null);

        // Assert
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Content.ShouldBe("form success");
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpResponseMessage>? _responseFactory;
        private readonly HttpResponseMessage? _staticResponse;

        public FakeHttpMessageHandler(HttpResponseMessage staticResponse)
        {
            _staticResponse = staticResponse;
        }

        public FakeHttpMessageHandler(Func<HttpResponseMessage>? responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_responseFactory != null)
                return Task.FromResult(_responseFactory());
            if (_staticResponse != null)
                return Task.FromResult(_staticResponse);
            throw new InvalidOperationException("No response configured for FakeHttpMessageHandler.");
        }
    }
}
