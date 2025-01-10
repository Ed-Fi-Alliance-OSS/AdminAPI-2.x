// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

public static class DeserializeHelper
{
    public static T? DeserializeOrReturn<T>(JsonNode request) where T : class
    {
        try
        {
            string jsonString = request.ToString();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
