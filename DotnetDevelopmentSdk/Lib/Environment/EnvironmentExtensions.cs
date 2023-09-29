// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.Extensions.Hosting;

namespace DotnetDevelopmentSdk.Lib.Environment;

public static class EnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment environment)
    {
        return environment.EnvironmentName.Equals("Test");
    }
}
