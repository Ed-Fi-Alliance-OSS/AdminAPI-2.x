// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public interface IAddApiClientCommand
{
    AddApiClientResult Execute(IAddApiClientModel apiClientModel, IOptions<AppSettings> options);
}

public class AddApiClientCommand : IAddApiClientCommand
{
    private readonly IUsersContext _usersContext;

    public AddApiClientCommand(IUsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public AddApiClientResult Execute(IAddApiClientModel apiClientModel, IOptions<AppSettings> options)
    {

        var vendor = _usersContext.Vendors.Include(x => x.Users)
            .Single(v => v.VendorId == apiClientModel.VendorId);

        var user = vendor.Users.FirstOrDefault();

        var application = _usersContext.Applications
            .Single(a => a.ApplicationId == apiClientModel.ApplicationId);

        var apiClient = new ApiClient(false)
        {
            Name = apiClientModel.Name,
            Key = apiClientModel.Key,
            Secret = apiClientModel.Secret,
            IsApproved = true,
            Application = application,
            UseSandbox = false,
            KeyStatus = "Active",
            SecretIsHashed = apiClientModel.SecretIsHashed,
            User = user,
        };

        _usersContext.ApiClients.Add(apiClient);

        _usersContext.SaveChanges();

        return new AddApiClientResult
        {
            Id = apiClient.ApiClientId,
            ApplicationId = application.ApplicationId,
            Key = apiClient.Key,
            Secret = apiClient.Secret,
            Name = apiClient.Name,
        };
    }
}

public interface IAddApiClientModel
{
    string Name { get; }
    string Key { get; }
    string Secret { get; }
    bool IsApproved { get; }
    int ApplicationId { get; }
    string KeyStatus { get; }
    bool SecretIsHashed { get; }
    int VendorId { get; }
}
public class AddApiClientResult
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}
