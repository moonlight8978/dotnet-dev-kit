// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Blueprint;

public partial class BlueprintData
{
    public BlueprintInfo Blueprint { get; set; } = null!;
    public Dictionary<string, string> BlueprintNameToCsv { get; set; } = new();
}
