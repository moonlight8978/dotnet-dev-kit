// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

public interface ICsvDataProvider
{
    string? GetColumnValue(string columnName);
    List<string> GetDictionaryKeys(string propertyName);
}
