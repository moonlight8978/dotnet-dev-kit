// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public class StringFlatTypeConverter : BaseCsvFlatTypeConverter
{
    public StringFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider) : base(csvTypeConverterProvider)
    {
    }

    public override List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        var stringValue = (string?)value;
        return new List<KeyValuePair<string, string>> { new(propertyName, stringValue ?? "") };
    }

    public override object? ToOriginal(string propertyName, string? value, Type typeInfo, ICsvDataProvider csvDataProvider)
    {
        return value;
    }
}
