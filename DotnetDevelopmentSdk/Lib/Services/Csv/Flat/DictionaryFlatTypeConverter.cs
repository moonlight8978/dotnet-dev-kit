// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections;
using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public class DictionaryFlatTypeConverter : BaseCsvFlatTypeConverter
{
    public DictionaryFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider) : base(
        csvTypeConverterProvider)
    {
    }

    public override List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        var propertyNameToValue = (IDictionary)value!;

        var keyType = typeInfo.GetGenericArguments()[0];
        var valueType = typeInfo.GetGenericArguments()[1];

        var keyConverter = CsvTypeConverterProvider.Flat(keyType);
        var valueConverter = CsvTypeConverterProvider.Flat(valueType);

        var propertyKeyValuePairs = new List<KeyValuePair<string, string>>();

        foreach (DictionaryEntry propertyNameAndValue in propertyNameToValue)
        {
            var propertyNameKeyValuePair =
                keyConverter.ToPropertyKeyValuePairs("", propertyNameAndValue.Key, keyType).First();
            propertyKeyValuePairs.AddRange(valueConverter.ToPropertyKeyValuePairs(
                $"{propertyName}_{propertyNameKeyValuePair.Value}",
                propertyNameAndValue.Value, valueType));
        }

        return propertyKeyValuePairs;
    }

    public override object ToOriginal(string propertyName, string? value, Type typeInfo,
        ICsvDataProvider csvDataProvider)
    {
        var keyType = typeInfo.GetGenericArguments()[0];
        var valueType = typeInfo.GetGenericArguments()[1];

        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var keyToValue = (IDictionary)Activator.CreateInstance(dictionaryType)!;

        var keyValues = csvDataProvider.GetDictionaryKeys(propertyName);

        var keyTypeConverter = CsvTypeConverterProvider.Flat(keyType);
        var valueTypeConverter = CsvTypeConverterProvider.Flat(valueType);

        foreach (var keyValue in keyValues)
        {
            var keyTypedValue = keyTypeConverter.ToOriginal("", keyValue, keyType, csvDataProvider)!;

            var columnName = $"{propertyName}_{keyValue}";
            string? columnValue;

            try
            {
                columnValue = csvDataProvider.GetColumnValue(columnName);
            }
            catch (Exception)
            {
                columnValue = "";
            }

            var valueTypedValue = valueTypeConverter.ToOriginal(columnName, columnValue, valueType, csvDataProvider);
            if (valueTypedValue != null)
            {
                keyToValue.Add(keyTypedValue, valueTypedValue);
            }
        }

        if (value == null)
        {
            return keyToValue;
        }

        return keyToValue;
    }
}
