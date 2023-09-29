// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Cors;

[CustomConfiguration("Cors")]
public class CorsConfiguration : ICustomConfiguration
{
    public string[] AllowedHosts { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();
    public string[] AllowedExposedHeaders { get; set; } = Array.Empty<string>();
    public bool AllowCredentials { get; set; }
}
