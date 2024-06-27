// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation;

public class SwaggerDefaultParameterFilter : IOperationFilter
{
    private readonly IOptions<AppSettings> _settings;

    public SwaggerDefaultParameterFilter(IOptions<AppSettings> settings)
    {
        _settings = settings;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters)
        {
            if (parameter.Name.ToLower().Equals("offset"))
            {
                parameter.Description = "Indicates how many items should be skipped before returning results.";
                parameter.Schema.Default = new OpenApiString(_settings.Value.DefaultPageSizeOffset.ToString());
            }
            else if (parameter.Name.ToLower().Equals("limit"))
            {
                parameter.Description = "Indicates the maximum number of items that should be returned in the results.";
                parameter.Schema.Default = new OpenApiString(_settings.Value.DefaultPageSizeLimit.ToString());
            }
        }

        switch (context.MethodInfo.Name)
        {
            case "GetProfiles":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Profile Id";
                        }
                        else if (parameter.Name.ToLower().Equals("name"))
                        {
                            parameter.Description = "Use this field to filter by Profile Name";
                        }
                    }
                    break;
                }
            case "GetResourceClaims":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Resource Claim Id";
                        }
                        else if (parameter.Name.ToLower().Equals("name"))
                        {
                            parameter.Description = "Use this field to filter by Resource Claim Name";
                        }
                    }
                    break;
                }
            case "GetVendors":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Vendor Id";
                        }
                        else if (parameter.Name.ToLower().Equals("company"))
                        {
                            parameter.Description = "Use this field to filter by Company Name";
                        }
                        else if (parameter.Name.ToLower().Equals("namespaceprefixes"))
                        {
                            parameter.Description = "Use this field to filter by Namespace Prefix";
                        }
                        else if (parameter.Name.ToLower().Equals("contactname"))
                        {
                            parameter.Description = "Use this field to filter by Contact Name";
                        }
                        else if (parameter.Name.ToLower().Equals("contactemailaddress"))
                        {
                            parameter.Description = "Use this field to filter by Contact Email Address";
                        }
                    }
                    break;
                }
            case "GetOdsInstances":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Ods Instance Id";
                        }
                        else if (parameter.Name.ToLower().Equals("name"))
                        {
                            parameter.Description = "Use this field to filter by Ods Instance Name";
                        }
                    }
                    break;
                }
            case "GetClaimSets":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Claimset Id";
                        }
                        else if (parameter.Name.ToLower().Equals("name"))
                        {
                            parameter.Description = "Use this field to filter by Claimset Name";
                        }
                    }
                    break;
                }
            case "GetApplications":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Profile Id";
                        }
                        else if (parameter.Name.ToLower().Equals("applicationname"))
                        {
                            parameter.Description = "Use this field to filter by Application Name";
                        }
                        else if (parameter.Name.ToLower().Equals("claimsetname"))
                        {
                            parameter.Description = "Use this field to filter by Claimset Name";
                        }
                    }
                    break;
                }
            case "GetActions":
                {
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Name.ToLower().Equals("id"))
                        {
                            parameter.Description = "Use this field to filter by Actions Id";
                        }
                        else if (parameter.Name.ToLower().Equals("name"))
                        {
                            parameter.Description = "Use this field to filter by Actions Name";
                        }
                    }
                    break;
                }
        }
    }
}
