// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminConsole.DBTests;

internal class TestAdminConsoleOptionsMonitor : IOptionsMonitor<AdminConsoleSettings>
{
    private readonly AdminConsoleSettings _currentValue;

    public TestAdminConsoleOptionsMonitor(AdminConsoleSettings currentValue)
    {
        _currentValue = currentValue;
    }

    public AdminConsoleSettings CurrentValue => _currentValue;

    public AdminConsoleSettings Get(string name)
        => _currentValue;

    public IDisposable OnChange(Action<AdminConsoleSettings, string> listener)
        => null;
}
