// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApi.AdminConsole.Helpers;

public class CorsSettings
{
    public bool EnableCors { get; set; }
    public string[] AllowedOrigins { get; set; } = new string[0];
}
