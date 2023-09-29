// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

public interface ICsvTypeConverter
{
    List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value, Type typeInfo);
    object? ToOriginal(string propertyName, string? value, Type typeInfo, ICsvDataProvider csvDataProvider);
}
