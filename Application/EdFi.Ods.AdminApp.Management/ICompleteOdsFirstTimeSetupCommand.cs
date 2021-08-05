﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management
{
    public interface ICompleteOdsFirstTimeSetupCommand
    {
        Action ExtraDatabaseInitializationAction { get; set; }

        Task<bool> Execute(string odsInstanceName, CloudOdsClaimSet claimSet, ApiMode mode);
    }
}
