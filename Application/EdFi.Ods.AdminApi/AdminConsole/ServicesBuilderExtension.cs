// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using EdFi.Ods.AdminApi.AdminConsole.Helpers;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.AutoMapper;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Repositories;
using EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.AdminConsole;

public static class ServicesBuilderExtension
{
    public static void AddAdminConsoleServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AdminConsoleSettings>(builder.Configuration.GetSection("AdminConsoleSettings"));
        builder.Services.AddAutoMapper(typeof(AdminConsoleMappingProfile));

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        builder.Services.AddTransient<IEncryptionKeySettings>(sp => sp.GetService<IOptions<AdminConsoleSettings>>().Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        builder.Services.AddTransient<IEncryptionKeyResolver, OptionsEncryptionKeyResolver>();
        builder.Services.AddScoped<IEncryptionService, EncryptionService>();

        builder.RegisterAdminConsoleServices();
        builder.RegisterAdminConsoleValidators();
    }

    private static void RegisterAdminConsoleServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
        builder.Services.AddScoped(typeof(IQueriesRepository<>), typeof(QueriesRepository<>));
        foreach (var type in typeof(IMarkerForEdFiAdminConsoleManagement).Assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract && (type.IsPublic || type.IsNestedPublic))
            {
                var concreteClass = type;

                var interfaces = concreteClass.GetInterfaces().ToArray();

                if (concreteClass.Namespace != null)
                {
                    if (!concreteClass.Namespace.EndsWith("Commands") &&
                        !concreteClass.Namespace.EndsWith("Queries"))
                    {
                        continue;
                    }

                    if (interfaces.Length == 1)
                    {
                        var serviceType = interfaces.Single();
                        if (serviceType.FullName == $"{concreteClass.Namespace}.I{concreteClass.Name}")
                            builder.Services.AddScoped(serviceType, concreteClass);
                    }
                    else if (interfaces.Length == 0)
                    {
                        if (!concreteClass.Name.EndsWith("Command")
                            && !concreteClass.Name.EndsWith("Query")
                            && !concreteClass.Name.EndsWith("Service"))
                        {
                            continue;
                        }
                        builder.Services.AddScoped(concreteClass);
                    }
                }
            }
        }
    }

    private static void RegisterAdminConsoleValidators(this WebApplicationBuilder webApplicationBuilder)
    {
        // Fluent validation
        webApplicationBuilder.Services
            .AddValidatorsFromAssembly(typeof(IMarkerForEdFiAdminConsoleManagement).Assembly)
            .AddFluentValidationAutoValidation();
        ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression)
                    => memberInfo?
                        .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
    }
}
