// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetTestSdk.Lib.Components;
using NUnit.Framework;
using Serilog;

namespace DotnetTestSdk.Lib.TestSuit;

public class BaseUnitTestSuit
{
    protected ILogger Logger => ComponentManager.Get<LoggerTestComponent>().Logger;

    protected readonly ITestComponentManager ComponentManager;

    private readonly ITestComponent _componentManager;

    protected BaseUnitTestSuit()
    {
        var testComponentManager = new TestComponentManager();
        _componentManager = testComponentManager;
        ComponentManager = testComponentManager;

        ComponentManager.Add<LoggerTestComponent>(GetType());
    }

    [SetUp]
    public async Task InitializeAsync()
    {
        await _componentManager.InitializeAsync();
        await OnPrepareAsync();
    }

    protected virtual Task OnPrepareAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnCompleteAsync()
    {
        return Task.CompletedTask;
    }

    [TearDown]
    public async Task DisposeAsync()
    {
        await _componentManager.DisposeAsync();
        await OnCompleteAsync();
    }
}
