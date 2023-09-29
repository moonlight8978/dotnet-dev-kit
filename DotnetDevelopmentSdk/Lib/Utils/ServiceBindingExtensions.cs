// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Utils;

public interface ITypeDirectedScopeBindedService
{
}

public interface ITypeDirectedSingletonBindedService
{
}

public interface ITypeDirectedTransientBindedService
{
}

public class BindingTypeAttribute : Attribute
{
    public Type BindingType { get; }

    public BindingTypeAttribute(Type bindingType)
    {
        BindingType = bindingType;
    }
}

public static class ServiceBindingExtensions
{
    public static void BindTypeDirectedScopedServices(this IServiceCollection services)
    {
        foreach (var dataAccessObjectType in ReflectionUtils.TypesImplements<ITypeDirectedScopeBindedService>())
        {
            var bindingTypeAttribute =
                (BindingTypeAttribute?)Attribute.GetCustomAttribute(dataAccessObjectType,
                    typeof(BindingTypeAttribute));
            if (bindingTypeAttribute == null)
            {
                services.AddScoped(dataAccessObjectType);
            }
            else
            {
                services.AddScoped(bindingTypeAttribute.BindingType, dataAccessObjectType);
            }
        }
    }

    public static void BindTypeDirectedSingletonServices(this IServiceCollection services)
    {
        foreach (var dataAccessObjectType in ReflectionUtils.TypesImplements<ITypeDirectedSingletonBindedService>())
        {
            var bindingTypeAttribute =
                (BindingTypeAttribute?)Attribute.GetCustomAttribute(dataAccessObjectType,
                    typeof(BindingTypeAttribute));
            if (bindingTypeAttribute == null)
            {
                services.AddSingleton(dataAccessObjectType);
            }
            else
            {
                services.AddSingleton(bindingTypeAttribute.BindingType, dataAccessObjectType);
            }
        }
    }

    public static void BindTypeDirectedTransientServices(this IServiceCollection services)
    {
        foreach (var dataAccessObjectType in ReflectionUtils.TypesImplements<ITypeDirectedTransientBindedService>())
        {
            var bindingTypeAttribute =
                (BindingTypeAttribute?)Attribute.GetCustomAttribute(dataAccessObjectType,
                    typeof(BindingTypeAttribute));
            if (bindingTypeAttribute == null)
            {
                services.AddTransient(dataAccessObjectType);
            }
            else
            {
                services.AddTransient(bindingTypeAttribute.BindingType, dataAccessObjectType);
            }
        }
    }
}
