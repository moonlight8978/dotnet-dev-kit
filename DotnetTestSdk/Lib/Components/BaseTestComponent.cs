// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Serilog;

namespace DotnetTestSdk.Lib.Components;

public interface ITestComponent
{
    Task InitializeAsync();
    Task DisposeAsync();
}

public abstract class BaseTestComponent : ITestComponent
{
    private ILogger _logger = null!;

    protected readonly ITestComponentManager ComponentManager;

    protected BaseTestComponent(ITestComponentManager componentManager)
    {
        ComponentManager = componentManager;
    }

    public virtual async Task InitializeAsync()
    {
        _logger = ComponentManager.Get<LoggerTestComponent>().Logger;
        _logger.Debug($"{GetType().Name} Initializing");
        await OnPrepareAsync();
        _logger.Debug($"{GetType().Name} Initialized");
    }

    public virtual async Task DisposeAsync()
    {
        _logger.Debug($"{GetType().Name} Cleaning");
        await OnCleanupAsync();
        _logger.Debug($"{GetType().Name} Cleaned");
    }

    protected abstract Task OnPrepareAsync();

    protected abstract Task OnCleanupAsync();
}
