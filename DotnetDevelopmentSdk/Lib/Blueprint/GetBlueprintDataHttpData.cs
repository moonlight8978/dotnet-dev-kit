// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using HybridModelBinding;

namespace DotnetDevelopmentSdk.Lib.Blueprint;

/// <summary>
/// /api/v1/projects/:projectSlug/blueprints/:version/data?hash=0834fcc44e856e698789d883e71339fb
/// /api/v1/projects/:projectSlug/blueprints/:version/data?hash=
/// </summary>
public partial class GetBlueprintDataRequestData
{
    [HybridBindProperty(Source.Route)] public string Project { get; set; } = null!;

    [HybridBindProperty(Source.Route)] public string Version { get; set; } = null!;

    [HybridBindProperty(Source.QueryString)]
    public string Hash { get; set; } = string.Empty;
}

public partial class GetBlueprintDataResponseData : BlueprintData
{
}
