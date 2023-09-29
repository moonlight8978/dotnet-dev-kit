// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using HybridModelBinding;

namespace DotnetDevelopmentSdk.Lib.Blueprint;

/// <summary>
/// /api/v1/projects/:projectSlug/blueprints/:version/info
/// </summary>
public partial class GetBlueprintInfoRequestData
{
    [HybridBindProperty(Source.Route)] public string Project { get; set; } = null!;

    [HybridBindProperty(Source.Route)] public string Version { get; set; } = null!;
}

public partial class GetBlueprintInfoResponseData
{
    public BlueprintInfo Blueprint { get; set; } = null!;
}
