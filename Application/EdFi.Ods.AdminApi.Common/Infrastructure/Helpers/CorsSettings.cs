<<<<<<<< HEAD:Application/EdFi.Ods.AdminApi.AdminConsole/Helpers/CorsSettings.cs
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
========
// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApi.Common.Helpers;

public class CorsSettings
{
    public bool EnableCors { get; set; }
    public string[]? AllowedOrigins { get; set; }
}
>>>>>>>> a7de2d9b (Fix CORS settings):Application/EdFi.Ods.AdminApi.Common/Infrastructure/Helpers/CorsSettings.cs
