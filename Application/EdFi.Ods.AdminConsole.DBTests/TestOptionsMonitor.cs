// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminConsole.DBTests;

internal class TestOptionsMonitor : IOptionsMonitor<AppSettings>
{
    private readonly AppSettings _currentValue;

    public TestOptionsMonitor(AppSettings currentValue)
    {
        _currentValue = currentValue;
    }

    public AppSettings CurrentValue => _currentValue;

    public AppSettings Get(string name)
        => _currentValue;

    public IDisposable OnChange(Action<AppSettings, string> listener)
        => null;
}
