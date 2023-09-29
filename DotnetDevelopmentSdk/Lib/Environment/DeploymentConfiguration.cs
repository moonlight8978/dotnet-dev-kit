// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Environment;

[CustomConfiguration("Deployment")]
public class DeploymentConfiguration : ICustomConfiguration
{
    [Required] public string Branch { get; set; } = null!;
}
