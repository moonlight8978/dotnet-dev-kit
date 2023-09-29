// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Logging;
using Serilog;

namespace DotnetTestSdk.Lib.Components;

public class LoggerTestComponent : BaseTestComponent
{
    public ILogger Logger { get; }

    public LoggerTestComponent(ITestComponentManager componentManager, Type testContextType) : base(componentManager)
    {
        Log.Logger = new LoggerConfiguration()
            .AddAdvancedLog()
            .WriteTo.File(".log/TestRunner.log")
            .CreateLogger();

        Logger = Log.ForContext(testContextType);
    }

    protected override Task OnPrepareAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task OnCleanupAsync()
    {
        return Task.CompletedTask;
    }
}
