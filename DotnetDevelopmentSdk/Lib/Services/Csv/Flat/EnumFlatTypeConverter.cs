// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public class EnumFlatTypeConverter : BaseCsvFlatTypeConverter
{
    public EnumFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider) : base(csvTypeConverterProvider)
    {
    }

    public override List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        return base.ToPropertyKeyValuePairs(propertyName, value == null ? "" : ((Enum)value).ToString(), typeInfo);
    }

    public override object? ToOriginal(string propertyName, string? value, Type typeInfo,
        ICsvDataProvider csvDataProvider)
    {
        return string.IsNullOrWhiteSpace(value) ? null : Enum.Parse(typeInfo, value);
    }
}
