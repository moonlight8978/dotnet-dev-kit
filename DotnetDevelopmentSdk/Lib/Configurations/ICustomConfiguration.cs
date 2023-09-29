// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Configurations;

public interface ICustomConfiguration
{
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomConfigurationAttribute : Attribute
{
    public string[] AvailableNestedKeys { get; } = Array.Empty<string>();
    public string Key { get; }
    public Type? ValidatorType { get; }

    public CustomConfigurationAttribute(string key, Type? validator = null)
    {
        ValidatorType = validator;
        Key = key;
    }

    public CustomConfigurationAttribute(string key, string[] availableNestedKeys, Type? validator = null) : this(key,
        validator)
    {
        AvailableNestedKeys = availableNestedKeys;
    }
}
