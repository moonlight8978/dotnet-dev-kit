// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections;
using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public class ListFlatTypeConverter : BaseCsvFlatTypeConverter
{
    private readonly string _delimiter;

    public ListFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider, string delimiter = ",") : base(
        csvTypeConverterProvider)
    {
        _delimiter = delimiter;
    }

    public override List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        if (value == null)
        {
            return new List<KeyValuePair<string, string>> { new(propertyName, "") };
        }

        var values = (IList)value;

        var itemType = typeInfo.GetGenericArguments()[0];

        if (!(itemType.IsPrimitive || itemType.IsEnum))
        {
            throw new NotSupportedException($"Type {itemType.FullName} not supported while using Flat list type");
        }

        var itemConverter = CsvTypeConverterProvider.Flat(itemType);

        var itemStringValues = (from object? itemValue in values
                                select itemConverter.ToPropertyKeyValuePairs(propertyName, itemValue, itemType)[0].Value).ToList();

        return new List<KeyValuePair<string, string>> { new(propertyName, string.Join(_delimiter, itemStringValues)) };
    }

    public override object ToOriginal(string propertyName, string? value, Type typeInfo,
        ICsvDataProvider csvDataProvider)
    {
        var itemType = typeInfo.GenericTypeArguments[0];
        var listType = typeof(List<>).MakeGenericType(itemType);
        var items = (IList)Activator.CreateInstance(listType)!;

        if (value == null)
        {
            return items;
        }

        var itemTypeConverter = CsvTypeConverterProvider.Flat(itemType);

        foreach (var item in value.Split(_delimiter))
        {
            items.Add(itemTypeConverter.ToOriginal(propertyName, item, itemType, csvDataProvider));
        }

        return items;
    }
}
