// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApi.Infrastructure;
using EdFi.Ods.AdminApi.Infrastructure.Database.Queries;
using EdFi.Security.DataAccess.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApi.DBTests.Database.QueryTests;

[TestFixture]
public class GetResourceClaimActionsQueryTests : SecurityDataTestBase
{
    [Test]
    public void ShouldGetResourceClaimActions()
    {
        var skip = 0;
        ResourceClaimAction[] results = null;
        using var securityContext = TestContext;
        var actions = SetupActions().Select(s => s.ActionId).ToArray();
        var resourceClaimId = SetupResourceClaims().FirstOrDefault().ResourceClaimId;
        var testResourceClaimActions = SetupResourceClaimActions(actions, resourceClaimId);
        var query = new GetResourceClaimActionsQuery(securityContext, Testing.GetAppSettings());
        results = query.Execute(new CommonQueryParams(skip, Testing.DefaultPageSizeLimit)).ToArray();
        results.Length.ShouldBe(testResourceClaimActions.Count);
        results.Select(x => x.ValidationRuleSetName).ShouldBe(testResourceClaimActions.Select(x => x.ValidationRuleSetName), true);
        results.Select(x => x.ResourceClaimId).ShouldBe(testResourceClaimActions.Select(x => x.ResourceClaimId), true);
        results.Select(x => x.ActionId).ShouldBe(testResourceClaimActions.Select(x => x.ActionId), true);
    }

    [Test]
    public void ShouldGetAllResourceClaimActions_With_Offset_and_Limit()
    {
        var offset = 1;
        var limit = 2;

        ResourceClaimAction[] results = null;
        using var securityContext = TestContext;
        //Set actions
        var actions = SetupActions().Select(s => s.ActionId).ToArray();
        //Set resourceClaims
        var resourceClaimId = SetupResourceClaims().FirstOrDefault().ResourceClaimId;

        //Add ResourceClaimActions
        var testResourceClaimActions = SetupResourceClaimActions(actions, resourceClaimId);
        var query = new GetResourceClaimActionsQuery(securityContext, Testing.GetAppSettings());
        results = query.Execute(new CommonQueryParams(offset, limit)).ToArray();

        results.Length.ShouldBe(2);
        results[0].ValidationRuleSetName.ShouldBe("Test2");
        results[1].ValidationRuleSetName.ShouldBe("Test3");
    }

    private IReadOnlyCollection<ResourceClaimAction> SetupResourceClaimActions(int[] actions, int resourceClaimId)
    {
        var resourceClaimActions = new List<ResourceClaimAction>();
        var resourceClaimCount = actions.Length;
        foreach (var index in Enumerable.Range(1, resourceClaimCount))
        {
            var resourceClaim = new ResourceClaimAction
            {
                ActionId = actions[index - 1],
                ResourceClaimId = resourceClaimId,
                ValidationRuleSetName = $"Test{index}"
            };
            resourceClaimActions.Add(resourceClaim);
        }
        Save(resourceClaimActions.Cast<object>().ToArray());
        return resourceClaimActions;
    }

    private IReadOnlyCollection<ResourceClaim> SetupResourceClaims(int resourceClaimCount = 1)
    {
        var resourceClaims = new List<ResourceClaim>();
        foreach (var index in Enumerable.Range(1, resourceClaimCount))
        {
            var resourceClaim = new ResourceClaim
            {
                ClaimName = $"TestResourceClaim{index:N}",
                ResourceName = $"TestResourceClaim{index:N}",
            };
            resourceClaims.Add(resourceClaim);
        }

        Save(resourceClaims.Cast<object>().ToArray());

        return resourceClaims;
    }

    private IReadOnlyCollection<Security.DataAccess.Models.Action> SetupActions(int resourceClaimCount = 5)
    {
        var actions = new List<Security.DataAccess.Models.Action>();
        foreach (var index in Enumerable.Range(1, resourceClaimCount))
        {
            var action = new Security.DataAccess.Models.Action
            {
                ActionName = $"TestResourceClaim{index:N}",
                ActionUri = $"http://ed-fi.org/odsapi/actions/TestResourceClaim{index:N}"
            };
            actions.Add(action);
        }

        Save(actions.Cast<object>().ToArray());

        return actions;
    }
}
