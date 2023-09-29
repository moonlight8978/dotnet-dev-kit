// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Logging;

[CustomConfiguration("ApplicationLogger")]
public class ApplicationLoggerConfiguration : ICustomConfiguration
{
    [Required] public string Tag { get; set; } = null!;
    [Required] public string HealthCheckPath { get; set; } = null!;
    [Required] public string BuildInfoPath { get; set; } = null!;
    public bool FileLogging { get; set; } = false;
}
