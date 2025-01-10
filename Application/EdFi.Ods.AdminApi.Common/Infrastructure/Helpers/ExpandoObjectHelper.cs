// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;

public static class ExpandoObjectHelper
{
    public static ExpandoObject NormalizeExpandoObject(ExpandoObject expando)
    {
        var dictionary = (IDictionary<string, object?>)expando;
        var normalized = new ExpandoObject() as IDictionary<string, object?>;

        foreach (var kvp in dictionary)
        {
            if (kvp.Value is JsonElement jsonElement)
            {
                normalized[kvp.Key] = jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.Number => jsonElement.GetDecimal(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    JsonValueKind.Object => JsonConvert.DeserializeObject<ExpandoObject>(jsonElement.GetRawText()),
                    _ => jsonElement.ToString()
                };
            }
            else if (kvp.Value is ExpandoObject nestedExpando)
            {
                normalized[kvp.Key] = NormalizeExpandoObject(nestedExpando);
            }
            else
            {
                normalized[kvp.Key] = kvp.Value;
            }
        }

        return (ExpandoObject)normalized;
    }

    public static JsonNode? FormatJson(ExpandoObject expandoObject)
    {
        var cleanedDocument = NormalizeExpandoObject(expandoObject);

        var document = JsonConvert.SerializeObject(cleanedDocument, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            Converters = new List<JsonConverter> { new ExpandoObjectConverter() },
            Formatting = Formatting.Indented
        });

        return JsonNode.Parse(document);
    }
}
