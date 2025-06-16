// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApi.Common.Infrastructure;
using EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;
using EdFi.Ods.AdminApi.Common.Settings;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Database.Commands;

public interface IAddApplicationCommand
{
    AddApplicationResult Execute(IAddApplicationModel applicationModel, IOptions<AppSettings> options);
}

public class AddApplicationCommand : IAddApplicationCommand
{
    private readonly IUsersContext _usersContext;

    public AddApplicationCommand(IUsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public AddApplicationResult Execute(IAddApplicationModel applicationModel, IOptions<AppSettings> options)
    {
        if (options.Value.PreventDuplicateApplications)
        {
            ValidateApplicationExistsQuery validateApplicationExists = new(_usersContext);
            bool applicationExists = validateApplicationExists.Execute(applicationModel);
            if (applicationExists)
            {
                var adminApiException = new AdminApiException("The Application already exists")
                {
                    StatusCode = HttpStatusCode.Conflict
                };
                throw adminApiException;
            }
        }
        var profiles = applicationModel.ProfileIds != null
           ? _usersContext.Profiles.Where(p => applicationModel.ProfileIds!.Contains(p.ProfileId))
           : null;

        var vendor = _usersContext.Vendors.Include(x => x.Users)
            .Single(v => v.VendorId == applicationModel.VendorId);

        var applicationEdOrgs = applicationModel.EducationOrganizationIds == null
            ? []
            : applicationModel.EducationOrganizationIds.Select(id => new ApplicationEducationOrganization
            {
                EducationOrganizationId = id
            });

        var application = new Application
        {
            ApplicationName = applicationModel.ApplicationName,
            ApplicationEducationOrganizations = new List<ApplicationEducationOrganization>(applicationEdOrgs),
            ClaimSetName = applicationModel.ClaimSetName,
            Profiles = [],
            Vendor = vendor,
            OperationalContextUri = OperationalContext.DefaultOperationalContextUri
        };

        if (profiles != null)
        {
            foreach (var profile in profiles)
            {
                application.Profiles.Add(profile);
            }
        }

        _usersContext.Applications.Add(application);

        _usersContext.SaveChanges();

        return new AddApplicationResult
        {
            ApplicationId = application.ApplicationId,
        };
    }
}

public interface IAddApplicationModel
{
    string? ApplicationName { get; }
    int VendorId { get; }
    string? ClaimSetName { get; }
    IEnumerable<int>? ProfileIds { get; }
    IEnumerable<long>? EducationOrganizationIds { get; }
}

public class AddApplicationResult
{
    public int ApplicationId { get; set; }
}
