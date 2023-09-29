// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetTestSdk.Lib.Components;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestSdk.Lib.TestSuit;

public abstract class BaseIntegrationTestSuit<TEnvironmentTestComponent, TExternalApiTestComponent> : BaseUnitTestSuit
    where TEnvironmentTestComponent : IEnvironmentTestComponent
    where TExternalApiTestComponent : ExternalApiTestComponent
{
    protected BaseIntegrationTestSuit()
    {
        ComponentManager.Add<TExternalApiTestComponent>();
        ComponentManager.Add<IEnvironmentTestComponent, TEnvironmentTestComponent>();
    }

    protected T GetService<T>() where T : notnull
    {
        return ComponentManager.Get<IEnvironmentTestComponent>().ServiceProvider.GetRequiredService<T>();
    }

    protected TEnvironmentTestComponent Environment =>
        (TEnvironmentTestComponent)ComponentManager.Get<IEnvironmentTestComponent>();
}
