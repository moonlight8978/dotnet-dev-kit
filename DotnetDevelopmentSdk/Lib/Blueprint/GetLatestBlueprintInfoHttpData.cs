// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using HybridModelBinding;

namespace DotnetDevelopmentSdk.Lib.Blueprint;

public class GetLatestBlueprintInfoRequestData
{
    [HybridBindProperty(Source.QueryString)]
    public string Feature { get; set; } = null!;

    [HybridBindProperty(Source.Route)]
    public string Project { get; set; } = null!;
}

public class GetLatestBlueprintInfoResponseData
{
    public BlueprintInfo Blueprint { get; set; } = null!;
}
