// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv.Flat;

public class UserDefinedClassFlatTypeConverter : BaseCsvFlatTypeConverter
{
    public UserDefinedClassFlatTypeConverter(CsvTypeConverterProvider csvTypeConverterProvider) : base(
        csvTypeConverterProvider)
    {
    }

    public override List<KeyValuePair<string, string>> ToPropertyKeyValuePairs(string propertyName, object? value,
        Type typeInfo)
    {
        var memberInfos = typeInfo.GetAllFieldAndProperties();
        return memberInfos.SelectMany(memberInfo =>
        {
            try
            {
                var propertyValue =
                    typeInfo.GetProperty(memberInfo._memberName, memberInfo._memberType)!.GetValue(value);
                var propertyCsvTypeConverter = CsvTypeConverterProvider.Flat(memberInfo._memberType);
                return propertyCsvTypeConverter.ToPropertyKeyValuePairs(
                    string.IsNullOrWhiteSpace(propertyName)
                        ? memberInfo._memberName
                        : $"{propertyName}_{memberInfo._memberName}",
                    propertyValue, memberInfo._memberType);
            }
            catch (Exception e)
            {
                throw new Exception($"{typeInfo.Name} - {memberInfo._memberName}- {e}");
            }
        }).ToList();
    }

    public override object ToOriginal(string propertyName, string? value, Type typeInfo,
        ICsvDataProvider csvDataProvider)
    {
        var memberInfos = typeInfo.GetAllFieldAndProperties();
        var record = Activator.CreateInstance(typeInfo)!;
        foreach (var memberInfo in memberInfos)
        {
            var memberTypeConverter = CsvTypeConverterProvider.Flat(memberInfo._memberType);
            if (memberInfo._memberType.IsPrimitive || memberInfo._memberType.IsEnum ||
                memberInfo._memberType == typeof(string))
            {
                var memberColumnName = string.IsNullOrWhiteSpace(propertyName)
                    ? memberInfo._memberName
                    : $"{propertyName}_{memberInfo._memberName}";
                var memberValue = csvDataProvider.GetColumnValue(string.IsNullOrWhiteSpace(propertyName)
                    ? memberInfo._memberName
                    : $"{propertyName}_{memberInfo._memberName}");
                var memberTypedValue = memberTypeConverter.ToOriginal(memberColumnName, memberValue,
                    memberInfo._memberType,
                    csvDataProvider);
                if (memberTypedValue != null)
                {
                    memberInfo._setValue(record, memberTypedValue);
                }
            }
            else
            {
                var memberColumnName = string.IsNullOrWhiteSpace(propertyName)
                    ? memberInfo._memberName
                    : $"{propertyName}_{memberInfo._memberName}";
                var memberTypedValue =
                    memberTypeConverter.ToOriginal(memberColumnName, null, memberInfo._memberType, csvDataProvider);
                if (memberTypedValue != null)
                {
                    memberInfo._setValue(record, memberTypedValue);
                }
            }
        }

        return record;
    }
}
