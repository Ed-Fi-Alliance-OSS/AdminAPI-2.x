// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApi.Common.Settings;

public class AdminConsoleSettings : IEncryptionKeySettings
{
    public CorsSettings CorsSettings { get; set; } = new CorsSettings();
    public string EncryptionKey { get; set; } = string.Empty;
    public string VendorCompany { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
}
