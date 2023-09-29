// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Sylvan.Data;
using Sylvan.Data.Csv;

namespace DotnetDevelopmentSdk.Lib.Services.Csv;

public class FlatCsvExporter
{
    private readonly CsvTypeConverterProvider _csvTypeConverterProvider;

    public FlatCsvExporter(CsvTypeConverterProvider csvTypeConverterProvider)
    {
        _csvTypeConverterProvider = csvTypeConverterProvider;
    }

    public async Task Export<T>(IEnumerable<T> rows, string filename) where T : class
    {
        await using var csvWriter = CsvDataWriter.Create(filename);

        var columnNames = new HashSet<string>();
        var columnNameToValueItems = rows.Select(row =>
        {
            var columnNameToValue = new Dictionary<string, string>();
            var keyValuePairs = _csvTypeConverterProvider.Flat(typeof(T)).ToPropertyKeyValuePairs("", row, typeof(T));
            keyValuePairs.ForEach(propertyNameAndValue =>
            {
                var (csvColumnName, csvColumnValue) = propertyNameAndValue;
                columnNames.Add(csvColumnName);
                columnNameToValue.Add(csvColumnName, csvColumnValue);
            });

            return columnNameToValue;
        }).ToList();

        var dataReader = ObjectDataReader.Build<Dictionary<string, string>>();

        dataReader = columnNames.Aggregate(dataReader,
            (current, columnName) => current.AddColumn(columnName,
                columnNameToValue =>
                    columnNameToValue.TryGetValue(columnName, out var columnValue) ? columnValue : ""));

        var data = dataReader.Build(columnNameToValueItems);
        await csvWriter.WriteAsync(data);
    }
}

public static class CsvExtensions
{
    private static readonly Dictionary<Type, List<MemberInfo>> s_memberInfosCache = new();

    public static List<MemberInfo> GetAllFieldAndProperties(this Type typeInfo)
    {
        if (s_memberInfosCache.TryGetValue(typeInfo, out var results))
        {
            return results;
        }

        results = typeInfo.GetFields().Select(fieldInfo => new MemberInfo
        {
            _memberName = fieldInfo.Name,
            _memberType = fieldInfo.FieldType,
            _setValue = fieldInfo.SetValue,
            _getValue = fieldInfo.GetValue
        }).ToList();
        results.AddRange(typeInfo.GetProperties().Select(propertyInfo => new MemberInfo
        {
            _memberName = propertyInfo.Name,
            _memberType = propertyInfo.PropertyType,
            _setValue = propertyInfo.SetValue,
            _getValue = propertyInfo.GetValue
        }));

        return results;
    }
}

public class MemberInfo
{
    public string _memberName;
    public Type _memberType;
    public Action<object, object> _setValue;
    public Func<object, object> _getValue;
}
