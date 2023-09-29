// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace DotnetTestSdk.Lib.Components;

public class ConfigurationTestComponent : BaseTestComponent
{
    protected IConfigurationRoot _configurationRoot = null!;

    public ConfigurationTestComponent(ITestComponentManager componentManager) : base(componentManager)
    {
    }

    protected override Task OnPrepareAsync()
    {
        _configurationRoot = new ConfigurationBuilder().AddJsonFile("testsettings.json")
            .AddJsonFile("testsettings.Local.json", true)
            .AddEnvironmentVariables()
            .Build();

        return Task.CompletedTask;
    }

    protected override Task OnCleanupAsync()
    {
        return Task.CompletedTask;
    }
}
