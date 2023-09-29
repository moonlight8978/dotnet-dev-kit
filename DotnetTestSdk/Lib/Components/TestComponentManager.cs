// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetTestSdk.Lib.Components;

public interface ITestComponentManager
{
    void Add<TInterface, TImplement>(params object[] args) where TImplement : ITestComponent, TInterface;
    void Add<TInterface, TImplement>(int priority, params object[] args) where TImplement : ITestComponent, TInterface;
    void Add<T>(params object[] args) where T : ITestComponent;
    void Add<T>(int priotity, params object[] args) where T : ITestComponent;
    T Get<T>() where T : ITestComponent;
}

public class TestComponentManager : ITestComponent, ITestComponentManager
{
    private readonly Dictionary<Type, ITestComponent> _typeToComponent = new();
    private readonly Dictionary<Type, int> _typeToPriority = new();
    private List<Type> _initializedComponentTypes = new();

    public void Add<TInterface, TImplement>(int priority, params object[] args)
        where TImplement : ITestComponent, TInterface
    {
        if (_typeToComponent.ContainsKey(typeof(TInterface)))
        {
            return;
        }

        var component = (TImplement?)Activator.CreateInstance(typeof(TImplement), args.Prepend(this).ToArray());
        if (component == null)
        {
            throw new Exception($"Cannot create test component: {typeof(TImplement).FullName}");
        }

        _typeToPriority.Add(typeof(TInterface), priority);
        _typeToComponent.Add(typeof(TInterface), component);
    }

    public void Add<TInterface, TImplement>(params object[] args)
        where TImplement : ITestComponent, TInterface
    {
        Add<TInterface, TImplement>(999, args);
    }

    public void Add<T>(int priority, params object[] args) where T : ITestComponent
    {
        Add<T, T>(priority, args);
    }

    public void Add<T>(params object[] args) where T : ITestComponent
    {
        Add<T, T>(999, args);
    }

    public T Get<T>() where T : ITestComponent
    {
        return (T)_typeToComponent[typeof(T)];
    }

    public async Task InitializeAsync()
    {
        _initializedComponentTypes = _typeToPriority.OrderBy(pair => pair.Value).Select(pair => pair.Key).ToList();
        foreach (var testComponent in _initializedComponentTypes.Select(testComponentType =>
                     _typeToComponent[testComponentType]))
        {
            await testComponent.InitializeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        _initializedComponentTypes.Reverse();
        foreach (var testComponentType in _initializedComponentTypes)
        {
            try
            {
                var testComponent = _typeToComponent[testComponentType];
                await testComponent.DisposeAsync();
            }
            catch (Exception)
            {
                //
            }
        }
    }
}
