// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Blueprint;

[CustomConfiguration("Blueprint")]
public class BlueprintConfiguration : ICustomConfiguration
{
    public string FileType { get; set; }
    public string BaseDownloadPath { get; set; }
    public string BaseUrl { get; set; }
    public int TimeoutInMillisecond { get; set; }
    public bool LocalMode { get; set; }

    public string Project { get; set; }

    // TODO: Make Local settings nested
    public string LocalFolder { get; set; }
    public string LocalVersion { get; set; }
    public string LocalUrl { get; set; }
}
