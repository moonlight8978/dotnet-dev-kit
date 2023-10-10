// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public interface IWorkflowMiddleware
{
    public Task InitializeAsync(IWorkflowContext workflowContext);
    public Task FinalizeAsync(IWorkflowContext workflowContext);
}

public abstract class WorkflowMiddleware : IWorkflowMiddleware
{
    public virtual Task InitializeAsync(IWorkflowContext workflowContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        return Task.CompletedTask;
    }
}

public class WorkflowMiddlewareManager : IWorkflowMiddleware, ITypeDirectedScopeBindedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, IWorkflowMiddleware> _typeToMiddleware = new();
    private readonly List<Type> _middlewareTypes = new();

    public WorkflowMiddlewareManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public WorkflowMiddlewareManager Use<T>() where T : IWorkflowMiddleware
    {
        if (!_typeToMiddleware.ContainsKey(typeof(T)))
        {
            _typeToMiddleware[typeof(T)] = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
            _middlewareTypes.Add(typeof(T));
        }

        return this;
    }

    public T Get<T>() where T : IWorkflowMiddleware
    {
        return (T)_typeToMiddleware[typeof(T)];
    }
    
    public void Configure<T>(Action<T> configure) where T : IWorkflowMiddleware
    {
        configure(Get<T>());
    }

    public async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        foreach (var middlewareType in _middlewareTypes)
        {
            await _typeToMiddleware[middlewareType].InitializeAsync(workflowContext);
        }
    }

    public async Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        foreach (var middlewareType in _middlewareTypes.AsEnumerable().Reverse())
        {
            await _typeToMiddleware[middlewareType].FinalizeAsync(workflowContext);
        }
    }
}
