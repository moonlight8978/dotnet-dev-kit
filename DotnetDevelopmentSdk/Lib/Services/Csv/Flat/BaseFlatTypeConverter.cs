// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public abstract class BaseCsvFlatTypeConverter : ICsvTypeConverter
{
    protected readonly CsvTypeConverterProvider CsvTypeConverterProvider;

    protected BaseCsvFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider)
    {
        CsvTypeConverterProvider = csvTypeConverterProvider;
    }

    public virtual List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        return new List<KeyValuePair<string, string>> { new(propertyName, value?.ToString() ?? "") };
    }

    public virtual object? ToOriginal(string propertyName, string? value, Type typeInfo, ICsvDataProvider csvDataProvider)
    {
        return csvDataProvider.GetColumnValue(propertyName);
    }
}
